using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Dolphin : MonoBehaviour
{
    public DolphinParameters dolphinParameters;
    public LocalParameters localParameters;
    public MultiplayerData multiplayerData;
    public DolphinDatabase dolphinDatabase;
    public GameEvent updateSwimMultiplierEvent;
    public MaterialList skins;
    public Rigidbody rb;
    public SkinnedMeshRenderer skinRenderer;
    public Transform nameBar;
    public ParticleSystem particles;
    public string dolphinName;
    public Planet planet;
    public Combo combo;
    public List<int> splits = new List<int>();
    public bool inWater, swimming, sliding, menuDolphin;
    public InputAction inputAction2D, inputAction3D, inputActionTwist, inputActionViewMode;
    private Animator animator;
    public readonly Vector3 forwardVector = Vector3.left; // left is front facing vector for this model
    public readonly Vector3 sideVector = Vector3.forward;
    private DateTime lastTimeMinutesRetrieved;
    private Vector3 swimDirection, rollDirection;
    private float swimMultiplier, rollMultiplier;
    private Vector2 input2D, input3D;
    private float twistInput;
    public float timeLastComboEnd, timeLastWaterExit, timeLastSplitStart;
    private readonly float comboCooldown = 1f, comboExpiration = 5f;

    public class Combo
    {
        public LocalParameters lp;
        public int frontflips, backflips, rightflips, leftflips, twists, tailslides;

        public Combo(LocalParameters lp)
        {
            this.lp = lp;
        }

        public int GetComboScore()
        {
            List<float> x = new List<float>();

            if (frontflips > lp.framesPerPoint) x.Add((float)frontflips / lp.framesPerPoint);
            if (backflips > lp.framesPerPoint) x.Add((float)backflips / lp.framesPerPoint);
            if (rightflips > lp.framesPerPoint) x.Add((float)rightflips / lp.framesPerPoint);
            if (leftflips > lp.framesPerPoint) x.Add((float)leftflips / lp.framesPerPoint);
            if (twists > lp.framesPerPoint) x.Add((float)twists / lp.framesPerPoint);
            if (tailslides > lp.framesPerPoint) x.Add((float)tailslides / lp.framesPerPoint);

            if (x.Count == 0) return 0;

            float score = 1f;
            foreach (float f in x) score *= f;

            return Mathf.RoundToInt(score);
        }

        public string GetComboString()
        {
            string output = "";

            if (frontflips > 0) output += " x " + ((float)frontflips / lp.framesPerFlip).ToString("F1") + " " + nameof(frontflips);
            if (backflips > 0) output += " x " + ((float)backflips / lp.framesPerFlip).ToString("F1") + " " + nameof(backflips);
            if (rightflips > 0) output += " x " + ((float)rightflips / lp.framesPerSideFlip).ToString("F1") + " " + nameof(rightflips);
            if (leftflips > 0) output += " x " + ((float)leftflips / lp.framesPerSideFlip).ToString("F1") + " " + nameof(leftflips);
            if (twists > 0) output += " x " + ((float)twists / lp.framesPerTwist).ToString("F1") + " " + nameof(twists);
            if (tailslides > 0) output += " x " + tailslides.ToString("N0") + " " + nameof(tailslides);

            if (output != "") output = output.Substring(3, output.Length - 3);

            return output;
        }
    }

    private void Awake()
    {
        combo = new Combo(localParameters);
    }

    private void NewSplit()
    {
        splits.Add(0);

        timeLastSplitStart = Time.time;
    }

    private void UploadScores()
    {
        dolphinDatabase.UploadDolphinScore(this);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        lastTimeMinutesRetrieved = DateTime.Now;

        SetSwimMultiplier(1f);

        rollMultiplier = 1f;

        rb.mass = dolphinParameters.mass;

        rb.drag = dolphinParameters.drag;

        rb.angularDrag = dolphinParameters.angularDrag;

        animator = GetComponent<Animator>();

        if (SpawnDolphins.instance != null) transform.SetParent(SpawnDolphins.instance.transform);

        if (localParameters.gameMode == LocalParameters.GameModes.Menu)
        {
            localParameters.SetLocalDolphin(this);

            localParameters.menuDolphin = this;

            nameBar.gameObject.SetActive(false);

            menuDolphin = true;

            transform.position = localParameters.menuDolphinPosition;

            string savedName = PlayerPrefs.GetString("dolphin", "");

            if (savedName != "" && UIMenu.instance != null) UIMenu.instance.SearchForDolphin(savedName);
        }
        else if (localParameters.gameMode == LocalParameters.GameModes.Singleplayer)
        {
            inputActionViewMode.performed += context => localParameters.SetViewMode(context.ReadValue<float>());

            localParameters.SetLocalDolphin(this);

            nameBar.gameObject.SetActive(false);

            dolphinDatabase.UploadDolphin(this, 1);
        }
        else if (localParameters.gameMode == LocalParameters.GameModes.Multiplayer)
        {
            inputActionViewMode.performed += context => localParameters.SetViewMode(context.ReadValue<float>());

            multiplayerData.InitializeMultiplayerDolphin(this);

            nameBar.gameObject.SetActive(true);

            nameBar.GetComponentInChildren<TextMeshProUGUI>().text = dolphinName;

            if (localParameters.localDolphin == this) dolphinDatabase.UploadDolphin(this, 1);
        }

        if (localParameters.localDolphin == this && localParameters.gameMode != LocalParameters.GameModes.Menu)
        {
            InvokeRepeating(nameof(UploadScores), 60f * dolphinDatabase.uploadScoreEveryMinutes, 60f * dolphinDatabase.uploadScoreEveryMinutes);

            InvokeRepeating(nameof(NewSplit), 0f, localParameters.secondsPerSplit);
        }
    }

    private void OnEnable()
    {
        inputAction2D.Enable();

        inputAction3D.Enable();

        inputActionTwist.Enable();

        inputActionViewMode.Enable();
    }

    private void OnDisable()
    {
        inputAction2D.Disable();

        inputAction3D.Disable();

        inputActionTwist.Disable();

        inputActionViewMode.Disable();
    }

    private void Update()
    {
        if (localParameters.localDolphin == this)
        {
            GetInputs();

            UpdateAnimation();
        }
    }

    private void FixedUpdate()
    {
        if (localParameters.localDolphin == this)
        {
            if (!sliding) MoveDolphin();

            UpdateCombo();
        }

        if (localParameters.gameMode == LocalParameters.GameModes.Multiplayer) UpdateNamebar();
    }

    private void GetInputs()
    {
        twistInput = inputActionTwist.ReadValue<float>();

        if (twistInput != 0f) TryTailslide();

        input2D = inputAction2D.ReadValue<Vector2>();

        input3D = inputAction3D.ReadValue<Vector2>();

        swimming = input2D.y > 0f && inWater;

        swimDirection = input2D.y * (transform.rotation * forwardVector); // swim

        rollDirection = transform.rotation * new Vector3(0f, 0f, input2D.x + input3D.y); // roll

        if (input3D.x != 0f && planet != null) // turn
            rollDirection = .5f * input3D.x * (transform.position - planet.transform.position).normalized;
    }

    private void UpdateAnimation()
    {
        if (swimMultiplier > 1f)
        {
            if (particles.isStopped) particles.Play();

            ParticleSystem.EmissionModule emission = particles.emission;

            emission.rateOverTime = rb.velocity.magnitude;
        }
        else if (particles.isPlaying) particles.Stop();
        ParticleSystem.MainModule main = particles.main;
        main.startSpeed = swimMultiplier;

        if (sliding)
        {
            animator.SetTrigger("Slide");

            return;
        }

        if (twistInput == 0f)
        {
            if (inWater)
            {
                if (input2D.y == 0f) animator.SetTrigger("Idle");
                else animator.SetTrigger("Swim");
            }
            else
            {
                float roll = input2D.x + input3D.y;

                if (roll == 0f) animator.SetTrigger("Air");
                else if (roll > 0f) animator.SetTrigger("Front");
                else animator.SetTrigger("Back");
            }
        }
        else
        {
            animator.SetTrigger("Twist"); // update when add tailslide
        }
    }

    private void UpdateCombo()
    {
        if (Time.time - timeLastComboEnd < comboCooldown) return;
        if (inWater && !sliding && Time.time - timeLastWaterExit > comboExpiration) ResetCombo();

        if (twistInput > 0f && sliding) combo.tailslides++;
        if (twistInput > 0f && !sliding) combo.twists++;

        if (!inWater)
        {
            float roll = input2D.x + input3D.y;
            if (roll > 0f) combo.frontflips++;
            else if (roll < 0f) combo.backflips++;

            if (input3D.x > 0f) combo.rightflips++;
            else if (input3D.x < 0f) combo.leftflips++;
        }
    }

    private void MoveDolphin()
    {
        if (inWater) rb.AddForce(swimDirection * dolphinParameters.swimSpeed * swimMultiplier, ForceMode.Impulse);

        rb.AddTorque(rollDirection * dolphinParameters.rollSpeed * rollMultiplier, ForceMode.VelocityChange);

        if (planet != null && !menuDolphin) planet.ApplyForces(this);
    }

    private void UpdateNamebar()
    {
        Vector3 forwards = nameBar.position - Camera.main.transform.position;

        Vector3 updwards = transform.position - planet.transform.position;

        nameBar.position = localParameters.nameBarOffset * updwards.normalized + transform.position;

        nameBar.rotation = Quaternion.LookRotation(forwards, updwards);
    }

    public void WaterEntry()
    {
        inWater = true;

        Vector3 entry = transform.rotation * forwardVector;

        Vector3 velocity = rb.velocity;

        if (Vector3.Angle(entry, velocity) < localParameters.niceEntryMaxAngle)
        {
            SetSwimMultiplier(swimMultiplier + localParameters.multiplierIncreaseNiceEntry);

            rb.AddForce(-dolphinParameters.waterEntryForce * rb.velocity.normalized, ForceMode.VelocityChange);
        }
        else
        {
            ResetMutliplier();

            ResetCombo();
        }
    }

    public void WaterExit()
    {
        inWater = false;

        timeLastWaterExit = Time.time;

        rb.AddForce(dolphinParameters.waterExitForce * rb.velocity.normalized, ForceMode.Impulse);
    }

    public void ResetMutliplier()
    {
        SetSwimMultiplier(1f);

        rb.velocity *= .1f;
    }

    public void ResetCombo()
    {
        if (splits.Count == 0) return;

        splits[splits.Count - 1] += combo.GetComboScore();

        combo = new Combo(localParameters);

        timeLastComboEnd = Time.time;
    }

    private void TryTailslide()
    {
        if (planet != null)
        {
            float distanceFromCore = (transform.position - planet.transform.position).magnitude;

            float distanceBelowSurface = planet.waterRadius - distanceFromCore;

            float angle = Vector3.Angle(rb.velocity, transform.position - planet.transform.position);

            if (distanceBelowSurface < 3f && distanceBelowSurface > 0f
            && angle > 0f && angle < 90f && !sliding) StartCoroutine(WaterTailslide());

        }

        // check for star tailslide here
    }

    private IEnumerator WaterTailslide()
    {
        sliding = true;

        float speed = rb.velocity.magnitude;

        int direction = SetPositionRotationWaterTailslide();

        float currentVelocity = 0f;

        while (inputActionTwist.IsPressed() && speed > 0f)
        {
            transform.position += direction * (transform.rotation * forwardVector).normalized * Time.fixedDeltaTime * speed;

            SetPositionRotationWaterTailslide(direction);

            speed = Mathf.SmoothDamp(speed, 0f, ref currentVelocity, speed / dolphinParameters.tailSlideDecceleration);

            yield return new WaitForFixedUpdate();
        }

        yield return null;

        rb.isKinematic = false;

        rb.AddForce((transform.position - planet.transform.position).normalized
        * speed * dolphinParameters.tailSlideLaunchForce, ForceMode.Impulse);

        if (direction == -1) transform.Rotate(sideVector, 180f);

        timeLastWaterExit = Time.time; // so combo doesn't end early

        sliding = false;
    }

    private int SetPositionRotationWaterTailslide(int direction = 0)
    {
        Vector3 upwards = transform.position - planet.transform.position;

        Vector3 forwards = -Vector3.Cross(upwards, transform.rotation * sideVector);

        float angle = Vector3.Angle(forwards, transform.rotation * forwardVector);

        if (direction == 0)
        {
            if (angle < 90f) direction = 1;
            else direction = -1;
        }

        transform.Rotate(direction * sideVector, angle);

        transform.position = upwards.normalized * planet.waterRadius + planet.transform.position;

        return direction;
    }

    private IEnumerator StarTailslide()
    {
        yield return null;
    }

    public int GetSkin()
    {
        if (skins.materials.Contains(skinRenderer.sharedMaterial))
        {
            return skins.materials.IndexOf(skinRenderer.sharedMaterial);
        }
        else return 0;
    }

    public void SetSkin(int skin)
    {
        if (skin >= skins.materials.Count || skin < 0) skin = 0;

        skinRenderer.material = skins.materials[skin];
    }

    public int GetMinutes()
    {
        int minutes = 0;

        minutes = Mathf.RoundToInt((float)((DateTime.Now - lastTimeMinutesRetrieved).TotalMinutes));

        lastTimeMinutesRetrieved = DateTime.Now;

        return minutes;
    }

    public int GetScores(out int score, out List<int> scoreSplits, out int superscore,
    out List<int> superscoreSplits, out int totalscore)
    {
        score = 0;
        scoreSplits = new List<int>();
        int finalIndex = Mathf.Max(0, splits.Count - localParameters.numberOfSplits);
        for (int i = splits.Count - 1; i >= finalIndex; i--)
        {
            scoreSplits.Add(splits[i]);
            score += splits[i];
        }

        superscore = 0;
        superscoreSplits = new List<int>();
        List<int> splitsCopy = new List<int>(localParameters.localDolphin.splits);
        splitsCopy.Sort();
        for (int i = splitsCopy.Count - 1; i >= finalIndex; i--)
        {
            superscoreSplits.Add(splitsCopy[i]);
            superscore += splitsCopy[i];
        }

        totalscore = 0;
        for (int i = 0; i < localParameters.localDolphin.splits.Count; i++)
            totalscore += localParameters.localDolphin.splits[i];

        return score;
    }

    public float GetSwimMultiplier() { return swimMultiplier; }

    private void SetSwimMultiplier(float value)
    {
        swimMultiplier = value;

        if (localParameters.localDolphin == this) updateSwimMultiplierEvent.Raise(value);
    }
}