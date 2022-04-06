using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Table : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public GridLayoutGroup columns, content;
    private List<GameObject> destroyOnClose = new List<GameObject>();

    public void SetTableInfo(string title, string data, List<int> columnsToInclude = null,
    float fontSize = 26f, float rowHeight = 40f)
    {
        DeleteExistingText();

        titleText.text = title;

        string[] rows = data.Split('\n');

        string[] columnTitles = rows[0].Split('/');

        if (columnsToInclude == null)
        {
            columnsToInclude = new List<int>();

            for (int i = 0; i < columnTitles.Length; i++) columnsToInclude.Add(i);
        }

        content.constraintCount = Mathf.Min(columnTitles.Length, columnsToInclude.Count);

        float width = columns.GetComponent<RectTransform>().rect.width / content.constraintCount;

        columns.cellSize = new Vector2(width, rowHeight);

        content.cellSize = columns.cellSize;

        for (int i = 0; i < columnTitles.Length; i++)
        {
            if (columnsToInclude.Contains(i)) CreateText(columnTitles[i], columns.transform, fontSize, FontStyles.Bold);
        }

        for (int i = 1; i < rows.Length; i++)
        {
            string[] cols = rows[i].Split('/');

            for (int j = 0; j < cols.Length; j++)
            {
                if (columnsToInclude.Contains(j)) CreateText(cols[j], content.transform, fontSize);
            }
        }
    }

    private void CreateText(string text, Transform parent, float fontSize, FontStyles fontStyle = FontStyles.Normal)
    {
        if (int.TryParse(text, out int number)) text = FormatNumber(number);

        if (DateTime.TryParseExact(text, "yyyy-MM-dd HH:mm:ss",
        System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.None, out DateTime date)) text = date.ToString("MM/dd/yyyy");

        GameObject prefab = new GameObject();

        GameObject g = Instantiate(prefab, parent);

        destroyOnClose.Add(g);

        TextMeshProUGUI t = g.AddComponent<TextMeshProUGUI>();

        t.fontSize = fontSize;

        t.alignment = TextAlignmentOptions.Center;

        t.fontStyle = fontStyle;

        t.enableWordWrapping = false;

        t.overflowMode = TextOverflowModes.Ellipsis;

        t.text = text;

        Destroy(prefab);
    }

    public void CloseTable()
    {
        DeleteExistingText();

        gameObject.SetActive(false);
    }

    private void DeleteExistingText()
    {
        foreach (GameObject g in destroyOnClose)
        {
            if (g != null) Destroy(g);
        }

        destroyOnClose.Clear();
    }

    public static string FormatNumber(int number)
    {
        if (number < 1e6) return number.ToString("N0");
        else if (number < 1e9) return (number / 1e6f).ToString("F3") + "M";
        else return (number / 1e9f).ToString("F3") + "B";
    }
}