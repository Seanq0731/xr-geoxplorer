/// <summary>
/// Centralized Unity tag names used by GeoXplorer app code.
/// Values must match ProjectSettings/TagManager.asset (except Coastlines — see note).
/// </summary>
public static class Tags
{
    public const string Flag = "flag";
    public const string FlagPrime = "flagPrime";
    public const string GoToTooltip = "GoToTooltip";
    public const string MapTile = "MapTile";
    public const string ARDefaultPlane = "ARDefaultPlane";
    public const string TooltipInteraction = "TooltipInteraction";
    public const string ARSessionOrigin = "ARSessionOrigin";
    public const string Tappable = "tappable";
    public const string OutcropTooltip = "OutcropTooltip";
    public const string AssetBundle = "AssetBundle";
    public const string TilePlane = "TilePlane";
    public const string InfoMarker = "InfoMarker";
    public const string NetworkRoom = "NetworkRoom";
    public const string AssetBundleLoader = "AssetBundleLoader";
    public const string SpotSphere = "SpotSphere";
    public const string TileStage = "TileStage";
    public const string ActiveModel = "activeModel";
    public const string MenuTitleText = "MenuTitleText";

    /// <summary>
    /// Assigned in BuildGlobe when instrument == "Coastlines".
    /// Not currently listed in TagManager.asset (existing behavior; do not add tags in #31).
    /// </summary>
    public const string Coastlines = "Coastlines";
}
