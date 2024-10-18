using System;

public static class RedDotFunction
{
    private static Random random = new Random();

    public static bool C()
    {
        return random.Next(2) == 1; // 0 또는 1을 생성하여 1일 경우 true, 0일 경우 false를 반환
    }

    public static bool D()
    {
        return random.Next(2) == 1; // 0 또는 1을 생성하여 1일 경우 true, 0일 경우 false를 반환
    }

    public static bool F()
    {
        return random.Next(2) == 1; // 0 또는 1을 생성하여 1일 경우 true, 0일 경우 false를 반환
    }

    public static bool G()
    {
        return random.Next(2) == 1; // 0 또는 1을 생성하여 1일 경우 true, 0일 경우 false를 반환
    }
}