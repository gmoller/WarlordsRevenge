using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WarlordsRevengeEditor
{
    public class TerrainList : IEnumerable<Terrain>
    {
        private readonly List<Terrain> _terrainList = new List<Terrain>();
        public ImageList ImageList { get; set; }
        public ListView ListView { get; set; }

        public void Add(Terrain terrain)
        {
            _terrainList.Add(terrain);
        }

        public IEnumerator<Terrain> GetEnumerator()
        {
            foreach (Terrain terrain in _terrainList)
            {
                yield return terrain;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct Terrain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
    }
}