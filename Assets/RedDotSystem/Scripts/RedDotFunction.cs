using System;

public static class RedDotFunction
{
    private static Random random = new Random();

    public static bool C()
    {
        return random.Next(2) == 1; // 0 �Ǵ� 1�� �����Ͽ� 1�� ��� true, 0�� ��� false�� ��ȯ
    }

    public static bool D()
    {
        return random.Next(2) == 1; // 0 �Ǵ� 1�� �����Ͽ� 1�� ��� true, 0�� ��� false�� ��ȯ
    }

    public static bool F()
    {
        return random.Next(2) == 1; // 0 �Ǵ� 1�� �����Ͽ� 1�� ��� true, 0�� ��� false�� ��ȯ
    }

    public static bool G()
    {
        return random.Next(2) == 1; // 0 �Ǵ� 1�� �����Ͽ� 1�� ��� true, 0�� ��� false�� ��ȯ
    }
}