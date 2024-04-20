using System;
using System.Linq;

namespace SMUBE.BattleState.Heatmap
{
    public static class HeatmapHelper
    {
        #region SingleHeatmapsOperations
        
        public static BaseHeatmap ClampHeatmap(BaseHeatmap heatmapA, int min, int max)
        {
            if (min > max)
                (min, max) = (max, min);
            
            var reprocessedHeatmap = ReprocessHeatmap(heatmapA, ClampProcessor);
            return reprocessedHeatmap;
            
            int ClampProcessor(int a)
            {
                if (a == -1) return -1;
                
                var result = a;
                result = Math.Min(min, result);
                result = Math.Max(max, result);
                return result;
            }
        }
        
        public static BaseHeatmap MultiplyHeatmap(BaseHeatmap heatmapA, int multiplier)
        {
            var reprocessedHeatmap = ReprocessHeatmap(heatmapA, MultiplyProcessor);
            return reprocessedHeatmap;
            
            int MultiplyProcessor(int a)
            {
                if (a == -1) return -1;
                
                return a * multiplier;
            }
        }
        
        #endregion
        
        #region TwoHeatmapsOperations

        public static BaseHeatmap AddHeatmaps(BaseHeatmap heatmapA, BaseHeatmap heatmapB)
        {
            var reprocessedHeatmap = ReprocessHeatmaps(heatmapA, heatmapB, AdditionProcessor);
            return reprocessedHeatmap;
            
            int AdditionProcessor(int a, int b)
            {
                if (a == -1 || b == -1) return -1;

                return a + b;
            }
        }
        
        public static BaseHeatmap SubtractHeatmaps(BaseHeatmap heatmapA, BaseHeatmap heatmapB)
        {
            var reprocessedHeatmap = ReprocessHeatmaps(heatmapA, heatmapB, SubtractionProcessor);
            return reprocessedHeatmap;
            
            int SubtractionProcessor(int a, int b)
            {
                if (a == -1 || b == -1) return -1;

                return a - b;
            }
        }
                
        public static BaseHeatmap MultiplyHeatmaps(BaseHeatmap heatmapA, BaseHeatmap heatmapB)
        {
            var reprocessedHeatmap = ReprocessHeatmaps(heatmapA, heatmapB, MultiplyProcessor);
            return reprocessedHeatmap;
            
            int MultiplyProcessor(int a, int b)
            {
                if (a == -1 || b == -1) return -1;

                return a * b;
            }
        }
        
        public static BaseHeatmap DivideHeatmaps(BaseHeatmap heatmapA, BaseHeatmap heatmapB)
        {
            var reprocessedHeatmap = ReprocessHeatmaps(heatmapA, heatmapB, DivideProcessor);
            return reprocessedHeatmap;
            
            int DivideProcessor(int a, int b)
            {
                if (a == -1 || b == -1) return -1;

                return b != 0 
                    ? a / b 
                    : a;
            }
        }

        #endregion
        
        private static BaseHeatmap ReprocessHeatmap(BaseHeatmap heatmapA, Func<int, int> processor)
        {
            var width = heatmapA.Heatmap.Count;
            var height = heatmapA.Heatmap.First().Count;
            
            var reprocessedHeatmap = new BaseHeatmap(width, height);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    reprocessedHeatmap.Heatmap[x][y] = processor.Invoke(heatmapA.Heatmap[x][y]);
                }
            }

            return reprocessedHeatmap;
        }
        
        public static BaseHeatmap ReprocessHeatmaps(BaseHeatmap heatmapA, BaseHeatmap heatmapB, Func<int, int, int> processor)
        {
            var width = heatmapA.Heatmap.Count;
            var height = heatmapA.Heatmap.First().Count;
            
            var reprocessedHeatmap = new BaseHeatmap(width, height);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    reprocessedHeatmap.Heatmap[x][y] = processor.Invoke(heatmapA.Heatmap[x][y], heatmapB.Heatmap[x][y]);
                }
            }

            return reprocessedHeatmap;
        }
        
        public static BaseHeatmap ReprocessHeatmaps(BaseHeatmap heatmapA, BaseHeatmap heatmapB, BaseHeatmap heatmapC, Func<int, int, int, int> processor)
        {
            var width = heatmapA.Heatmap.Count;
            var height = heatmapA.Heatmap.First().Count;
            
            var reprocessedHeatmap = new BaseHeatmap(width, height);
            reprocessedHeatmap.Prefill();
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    reprocessedHeatmap.Heatmap[x][y] = processor.Invoke(heatmapA.Heatmap[x][y], heatmapB.Heatmap[x][y], heatmapC.Heatmap[x][y]);
                }
            }

            return reprocessedHeatmap;
        }
    }
}