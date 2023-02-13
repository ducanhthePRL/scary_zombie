// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("nh0THCyeHRYenh0dHM5FSa5qQIRnfUc3zWuxvpA5xtOTdBxgidsoHOon9jt7ueRwoUYnV/WGcw2CERtrLJ4dPiwRGhU2mlSa6xEdHR0ZHB8VnHoiVDiBbaUUwgj+d3jc1tEinx342hMZN0OP+08rAyVE3AwcdjOhYxuDPEWAoJecnLMnfFM9GNgDgzzkrroIjT1COa23WX516bubdIi3ksIomYKnIy/Q7xJEKFpAqXHkuiuS4s6srMow5qudNCtujC94UjElqE7t/S6fDOauUWTyzJ5SQ6dnCHJ2Z3pua1Bhb62EC89I9m19WOrj7Flylesj/4/7POywIfg+HI31DezdegH2xRv9y6lK3dfLRAiQhpNz67RvbT2uyjBMZ3Gdvx4fHRwd");
        private static int[] order = new int[] { 1,5,13,5,5,9,9,10,8,10,12,11,12,13,14 };
        private static int key = 28;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
