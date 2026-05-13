// Assets/UI/Menus/Scripts/Editor/PauseMenuHierarchyFixer.cs
// Run via Unity menu: Tools > Pause Menu > Fix Hierarchy
// Fixes all RectTransform, color, and TMP issues on the pause menu hierarchy.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;

public static class PauseMenuHierarchyFixer
{
    // ── target values ────────────────────────────────────────────────────────
    const float PANEL_WIDTH   = 400f;
    const float PANEL_HEIGHT  = 520f;
    const float HEADER_HEIGHT = 44f;

    static readonly Color COLOR_HEADER       = new Color(0x8B / 255f, 0x00 / 255f, 0x00 / 255f, 1f); // #8B0000
    static readonly Color COLOR_PANEL        = new Color(0x0E / 255f, 0x0E / 255f, 0x0E / 255f, 1f); // #0E0E0E
    static readonly Color COLOR_TITLE_TEXT   = Color.white;

    // ── entry point ──────────────────────────────────────────────────────────
    [MenuItem("Tools/Pause Menu/Fix Hierarchy")]
    static void FixHierarchy()
    {
        // 1. Locate root canvas
        Canvas menuCanvas = FindCanvas("MenuCanvas");
        if (menuCanvas == null)
        {
            Log("ERROR: Could not find GameObject named 'MenuCanvas' with a Canvas component.");
            return;
        }

        // 2. Backdrop
        RectTransform backdrop = FindChildRect(menuCanvas.transform, "MenuBackdrop");
        if (backdrop == null) { Log("ERROR: MenuBackdrop not found under MenuCanvas."); return; }

        // 3. PauseMenuPanel
        RectTransform panel = FindChildRect(backdrop, "PauseMenuPanel");
        if (panel == null) { Log("ERROR: PauseMenuPanel not found under MenuBackdrop."); return; }

        // ── 7. PauseMenuPanel RectTransform ──────────────────────────────────
        FixPanelRect(panel);

        // ── 5a. PauseMenuPanel Image color ───────────────────────────────────
        FixImageColor(panel, "PauseMenuPanel", COLOR_PANEL);

        // ── 2. Confirm Header is a child of PauseMenuPanel ───────────────────
        RectTransform header = FindChildRect(panel, "Header");
        if (header == null)
        {
            Log("Header not found — creating it under PauseMenuPanel.");
            header = CreateChildImage(panel, "Header", COLOR_HEADER);
        }

        // ── 4. Check for extra Canvas on Header ───────────────────────────────
        RemoveNestedCanvas(header, "Header");

        // ── 1. Fix Header RectTransform ───────────────────────────────────────
        FixHeaderRect(header);

        // ── 5b. Header Image color (#8B0000) ──────────────────────────────────
        FixImageColor(header, "Header", COLOR_HEADER);

        // ── 3 & 6. Title (TextMeshPro) child of Header ───────────────────────
        RectTransform title = FindChildRect(header, "Title");
        if (title == null)
        {
            Log("Title not found — creating TextMeshPro child under Header.");
            title = CreateTitleTMP(header);
        }
        else
        {
            FixTitleRect(title);
            FixTitleTMP(title);
        }

        // ── 4. Check for extra Canvas on Title ────────────────────────────────
        RemoveNestedCanvas(title, "Title");

        // Mark scene dirty so Unity saves the changes
        EditorUtility.SetDirty(menuCanvas.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            menuCanvas.gameObject.scene);

        Log("Done. Save the scene (Ctrl+S) to persist changes.");
    }

    // ── fix methods ──────────────────────────────────────────────────────────

    static void FixPanelRect(RectTransform panel)
    {
        bool changed = false;

        // Center-anchor (0.5, 0.5) → (0.5, 0.5), no stretch
        if (panel.anchorMin != new Vector2(0.5f, 0.5f))
        { panel.anchorMin = new Vector2(0.5f, 0.5f); changed = true; }
        if (panel.anchorMax != new Vector2(0.5f, 0.5f))
        { panel.anchorMax = new Vector2(0.5f, 0.5f); changed = true; }
        if (panel.pivot != new Vector2(0.5f, 0.5f))
        { panel.pivot = new Vector2(0.5f, 0.5f); changed = true; }
        if (panel.anchoredPosition != Vector2.zero)
        { panel.anchoredPosition = Vector2.zero; changed = true; }
        if (panel.sizeDelta != new Vector2(PANEL_WIDTH, PANEL_HEIGHT))
        { panel.sizeDelta = new Vector2(PANEL_WIDTH, PANEL_HEIGHT); changed = true; }

        Log(changed ? "PauseMenuPanel: RectTransform corrected." : "PauseMenuPanel: RectTransform OK.");
    }

    // Header: top-stretch inside PauseMenuPanel, height = HEADER_HEIGHT
    // anchorMin=(0,1) anchorMax=(1,1)  anchoredPos=(0, -H/2)  sizeDelta=(0, H)
    static void FixHeaderRect(RectTransform header)
    {
        bool changed = false;

        Vector2 wantAnchorMin = new Vector2(0f, 1f);
        Vector2 wantAnchorMax = new Vector2(1f, 1f);
        Vector2 wantPivot     = new Vector2(0.5f, 0.5f);
        Vector2 wantPos       = new Vector2(0f, -HEADER_HEIGHT / 2f); // -22
        Vector2 wantSize      = new Vector2(0f, HEADER_HEIGHT);       //  44

        if (header.anchorMin != wantAnchorMin) { header.anchorMin = wantAnchorMin; changed = true; }
        if (header.anchorMax != wantAnchorMax) { header.anchorMax = wantAnchorMax; changed = true; }
        if (header.pivot     != wantPivot)     { header.pivot     = wantPivot;     changed = true; }
        if (header.anchoredPosition != wantPos) { header.anchoredPosition = wantPos; changed = true; }
        if (header.sizeDelta != wantSize)       { header.sizeDelta = wantSize;       changed = true; }

        Log(changed
            ? $"Header: RectTransform corrected → anchorMin={wantAnchorMin} anchorMax={wantAnchorMax} " +
              $"pos={wantPos} size={wantSize}"
            : "Header: RectTransform OK.");
    }

    // Title: full-stretch inside Header, zero offsets
    static void FixTitleRect(RectTransform title)
    {
        bool changed = false;

        if (title.anchorMin != Vector2.zero)          { title.anchorMin = Vector2.zero;          changed = true; }
        if (title.anchorMax != Vector2.one)           { title.anchorMax = Vector2.one;            changed = true; }
        if (title.pivot     != new Vector2(0.5f,0.5f)){ title.pivot = new Vector2(0.5f, 0.5f);   changed = true; }
        if (title.anchoredPosition != Vector2.zero)   { title.anchoredPosition = Vector2.zero;    changed = true; }
        if (title.sizeDelta != Vector2.zero)          { title.sizeDelta = Vector2.zero;           changed = true; }

        Log(changed ? "Title: RectTransform corrected (full-stretch)." : "Title: RectTransform OK.");
    }

    static void FixTitleTMP(RectTransform title)
    {
        TextMeshProUGUI tmp = title.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Log("Title: no TextMeshProUGUI found — adding one.");
            tmp = title.gameObject.AddComponent<TextMeshProUGUI>();
        }

        bool changed = false;

        if (tmp.text != "PAUSED")
        { tmp.text = "PAUSED"; changed = true; }

        if (tmp.color != COLOR_TITLE_TEXT)
        { tmp.color = COLOR_TITLE_TEXT; changed = true; }

        if (tmp.alignment != TextAlignmentOptions.Center)
        { tmp.alignment = TextAlignmentOptions.Center; changed = true; }

        if (tmp.fontSize < 1f)
        { tmp.fontSize = 20f; changed = true; }

        if (tmp.font == null)
            Log("WARNING: Title TextMeshPro has no Font Asset assigned — drag one in the Inspector.");
        else
            Log($"Title: font asset = {tmp.font.name}");

        Log(changed ? "Title: TMP settings corrected." : "Title: TMP settings OK.");
    }

    static void FixImageColor(RectTransform rt, string label, Color want)
    {
        UnityEngine.UI.Image img = rt.GetComponent<UnityEngine.UI.Image>();
        if (img == null)
        {
            Log($"{label}: no Image component found — adding one.");
            img = rt.gameObject.AddComponent<UnityEngine.UI.Image>();
        }

        if (img.color != want)
        {
            img.color = want;
            Log($"{label}: Image color set to {ColorUtility.ToHtmlStringRGBA(want)}.");
        }
        else
        {
            Log($"{label}: Image color OK ({ColorUtility.ToHtmlStringRGBA(want)}).");
        }
    }

    static void RemoveNestedCanvas(RectTransform rt, string label)
    {
        Canvas c = rt.GetComponent<Canvas>();
        if (c != null)
        {
            Log($"WARNING: {label} had an extra Canvas component — removing it (would break sort order).");
            Object.DestroyImmediate(c);

            // Also remove the GraphicRaycaster that pairs with it, if present
            UnityEngine.UI.GraphicRaycaster gr = rt.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (gr != null) Object.DestroyImmediate(gr);
        }
        else
        {
            Log($"{label}: no extra Canvas — OK.");
        }
    }

    // ── factory helpers ──────────────────────────────────────────────────────

    static RectTransform CreateChildImage(RectTransform parent, string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, $"Create {name}");

        RectTransform rt = go.AddComponent<RectTransform>();
        go.AddComponent<UnityEngine.UI.Image>().color = color;
        return rt;
    }

    static RectTransform CreateTitleTMP(RectTransform headerRect)
    {
        GameObject go = new GameObject("Title");
        go.transform.SetParent(headerRect, false);
        Undo.RegisterCreatedObjectUndo(go, "Create Title TMP");

        RectTransform rt = go.AddComponent<RectTransform>();

        // Full-stretch inside Header
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta        = Vector2.zero;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = "PAUSED";
        tmp.color     = COLOR_TITLE_TEXT;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize  = 20f;

        if (tmp.font == null)
            Log("Title created — WARNING: no Font Asset assigned yet. Drag a TMP font into the Title's Font Asset slot in the Inspector.");
        else
            Log($"Title created with font: {tmp.font.name}");

        return rt;
    }

    // ── utilities ────────────────────────────────────────────────────────────

    static Canvas FindCanvas(string name)
    {
#if UNITY_2023_1_OR_NEWER
        foreach (Canvas c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
#else
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
#endif
            if (c.gameObject.name == name) return c;
        return null;
    }

    static RectTransform FindChildRect(Transform parent, string childName)
    {
        Transform t = parent.Find(childName);
        return t != null ? t as RectTransform : null;
    }

    static void Log(string msg) => Debug.Log($"[PauseMenuHierarchyFixer] {msg}");
}
#endif
