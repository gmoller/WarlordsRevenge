using System;

namespace WarlordsRevengeEditor
{
    public class Cell
    {
        private readonly CellData[] _data;

        public Cell()
        {
            _data = new CellData[Layers.NumberOfLayers];
            for (int i = 0; i < Layers.NumberOfLayers; i++)
            {
                _data[i].PaletteId = 0;
                _data[i].TerrainId = -1;
            }
        }

        public void AddCellData(int layerId, int paletteId, int terrainId)
        {
            ValidateLayerParameter(layerId);

            _data[layerId - 1].PaletteId = paletteId;
            _data[layerId - 1].TerrainId = terrainId;
        }

        public void RemoveTerrain(int layerId)
        {
            ValidateLayerParameter(layerId);

            _data[layerId - 1].PaletteId = 0;
            _data[layerId - 1].TerrainId = -1;
        }

        public CellData GetCellData(int layerId)
        {
            ValidateLayerParameter(layerId);

            return _data[layerId - 1];
        }

        private void ValidateLayerParameter(int layerId)
        {
            if (layerId <= 0 || layerId > Layers.NumberOfLayers)
            {
                throw new ApplicationException(string.Format("Layer must be between 1 and {0}", Layers.NumberOfLayers));
            }
        }

        public bool IsEmpty()
        {
            bool empty = true;
            for (int i = 0; i < Layers.NumberOfLayers; i++)
            {
                if (_data[i].TerrainId >= 0)
                {
                    empty = false;
                }
            }

            return empty;
        }

        public override string ToString()
        {
            string data = string.Empty;
            for (int i = 0; i < Layers.NumberOfLayers; i++)
            {
                string s = string.Format("{0};{1}", _data[i].PaletteId, _data[i].TerrainId);
                data += s + '|';
            }

            data = data.TrimEnd('|');

            return data;
        }
    }

    public struct CellData
    {
        public int PaletteId { get; set; }
        public int TerrainId { get; set; }
    }
}