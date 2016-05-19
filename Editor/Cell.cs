using System.Collections.Generic;
using System.Linq;

namespace WarlordsRevengeEditor
{
    public class Cell
    {
        private readonly List<int> _terrainIds = new List<int>();

        public void AddTerrainId(int terrainId)
        {
            if (_terrainIds.Count == 0)
            {
                _terrainIds.Add(terrainId);
            }
            else
            {
                if (_terrainIds[_terrainIds.Count - 1] == -1)
                {
                    _terrainIds[_terrainIds.Count - 1] = terrainId;
                }
                else
                {
                    _terrainIds.Add(terrainId);
                }
            }
        }

        public void RemoveTerrain()
        {
            if (_terrainIds.Count > 0)
            {
                _terrainIds.RemoveAt(_terrainIds.Count - 1);
            }
        }

        public int GetTerrainId(int layer)
        {
            if (_terrainIds.Count == layer)
            {
                return -1; // no terrain
            }

            return _terrainIds[layer];
        }

        public override string ToString()
        {
            if (_terrainIds.Count == 0)
            {
                return "-1";
            }

            string terrain = _terrainIds.Aggregate(string.Empty, (current, terrainId) => current + terrainId + "|");
            terrain = terrain.TrimEnd('|');

            return string.Format("{0}", terrain);
        }
    }
}