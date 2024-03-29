﻿using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Hercules.MA.GraphicEngine.Models.Graficas
{
    public abstract class GraficaBase
    {
        public abstract byte[] GenerateCSV();
        public bool hideLeyend { get; set; }
        public string groupId { get; set; }
        public bool isAbr { get; set; }
        public bool isPercentage { get; set; }
        public bool isDate { get; set; }
        public bool isHorizontal { get; set; }
        public bool isVertical { get; set; }
        public bool isNodes { get; set; }
    }

    public class Options
    {
        public Animation animation { get; set; }
        public Plugin plugins { get; set; }
        public Dictionary<string, Eje> scales { get; set; }
        public string indexAxis { get; set; }
    }

    public class Animation
    {
        public int duration { get; set; }
    }

    public class Plugin
    {
        public Title title { get; set; }
    }

    public class Title
    {
        public bool display { get; set; }
        public string text { get; set; }
    }

    public class Eje
    {
        public string position { get; set; }
        public Title title { get; set; }
    }
}
