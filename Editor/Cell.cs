using System;
using System.Globalization;

namespace WarlordsRevengeEditor
{
    public class Cell
    {
        private readonly int[] _terrainIds;

        public Cell()
        {
            _terrainIds = new int[Layers.NumberOfLayers];
            for (int i = 0; i < Layers.NumberOfLayers; i++)
            {
                _terrainIds[i] = -1;
            }
        }

        public void AddTerrainId(int layerId, int terrainId)
        {
            ValidateLayerParameter(layerId);

            _terrainIds[layerId - 1] = terrainId;
        }

        public void RemoveTerrain(int layerId)
        {
            ValidateLayerParameter(layerId);

            _terrainIds[layerId - 1] = -1;
        }

        public int GetTerrainId(int layerId)
        {
            ValidateLayerParameter(layerId);

            return _terrainIds[layerId - 1];
        }

        private void ValidateLayerParameter(int layerId)
        {
            if (layerId <= 0 || layerId > Layers.NumberOfLayers)
            {
                throw new ApplicationException(string.Format("Layer must be between 1 and {0}", Layers.NumberOfLayers));
            }
        }

        public override string ToString()
        {
            string terrain = string.Empty;
            for (int i = 0; i < Layers.NumberOfLayers; i++)
            {
                string s = _terrainIds[i].ToString(CultureInfo.InvariantCulture);
                terrain +=  s + '|';
            }

            terrain = terrain.TrimEnd('|');

            return string.Format("{0}", terrain);
        }
    }
}