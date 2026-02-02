using Game.Scripts.Data;
using UnityEditor;

#if UNITY_EDITOR
namespace Game.Scripts.Editor_Utils
{
    internal static class OfferDataFinder
    {
        public static OffersData FindCatalog()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(OffersData));
            if (guids == null || guids.Length == 0) return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<OffersData>(path);
        }
    }
}
#endif