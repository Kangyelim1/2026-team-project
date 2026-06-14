using System;

public static class AmmoEvents
{
    public static event Action<int, int, bool> OnAmmoChanged;

    public static void Notify(int current, int max, bool isReloading)
    {
        OnAmmoChanged?.Invoke(current, max, isReloading);
    }
}