namespace Code.Utils {
    public static class MathUtils {
        public static bool HasSameSign(float a, float b) {
            return a > 0 && b > 0 || a < 0 && b < 0;
        }
    }
}