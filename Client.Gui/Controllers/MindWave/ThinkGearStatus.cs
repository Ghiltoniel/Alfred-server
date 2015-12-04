namespace Alfred.Client.Gui.Controllers.MindWave
{
    class ThinkGearStatus
    {
        public bool? isAuthorized { get; set; }
        public int? poorSignalLevel { get; set; }
        public int? blinkStrength { get; set; }
        public ESense eSense { get; set; }

        public EegPower eegPower { get; set; }
    }

    class ESense
    {
        public int attention { get; set; }
        public int meditation { get; set; }
    }

    class EegPower
    {
        public int delta { get; set; }
        public int theta { get; set; }
        public int lowAlpha { get; set; }
        public int highAlpha { get; set; }
        public int lowBeta { get; set; }
        public int highBeta { get; set; }
        public int lowGamma { get; set; }
        public int highGamma { get; set; }
    }
}
