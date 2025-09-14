using System;

public interface IAmmoSource
{
    int CurrentAmmo {  get; }
    int MaxAmmo { get; }
    bool IsReloading { get; }
    event System.Action OnAmmoChanged;
}
