using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialList", menuName = "Material List")]
public class MaterialList : ScriptableObject
{
    public List<Material> materials;
}
