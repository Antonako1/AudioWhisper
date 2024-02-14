public static class Errorchecks{
    public static bool Errorcheck_volume(int value){
        return 0 <= value && value <= 100;
    }
    public static bool Errorcheck_over0(int value){
        return 0 <= value;
    }
    
}