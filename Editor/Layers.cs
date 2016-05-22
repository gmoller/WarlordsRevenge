using System.Collections.Generic;

namespace WarlordsRevengeEditor
{
    public class Layers
    {
        public const int NumberOfLayers = 6;

        public List<string> GetLayers()
        {
            var layers = new List<string>();
            for (int i = 1; i <= NumberOfLayers; i++)
            {
                layers.Add(string.Format("Layer {0}", i));
            }

            return layers;
        }
    }
}