using System.Collections.Generic;

namespace TheDungeon.AssetReferenceFinder {

    public class FindAllReferencesWindowResult {

        public FindAllReferencesWindowAsset Target { get; set; }

        public List<FindAllReferencesWindowAsset> Usages { get; } = new List<FindAllReferencesWindowAsset>(10);

    }

}
