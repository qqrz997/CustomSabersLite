﻿using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using CustomSabersLite.Data;
using IPA.Config.Stores.Attributes;
using UnityEngine;
using CustomSabersLite.Models;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomSabersLite.Configuration;

internal class CSLConfig
{
    public virtual bool Enabled { get; set; } = true;

    public virtual string CurrentlySelectedSaber { get; set; } = null;

    public virtual bool DisableWhiteTrail { get; set; } = true;

    public virtual bool OverrideTrailDuration { get; set; } = false;

    public virtual int TrailDuration { get; set; } = 100;

    public virtual bool OverrideTrailWidth { get; set; } = false;

    public virtual int TrailWidth { get; set; } = 100;

    public virtual TrailType TrailType { get; set; } = TrailType.Custom;

    public virtual bool EnableCustomEvents { get; set; } = true;

    [Ignore] public bool EnableMenuSabers { get; set; } = false;

    [Ignore] public OrderBy OrderByFilter { get; set; } = OrderBy.Name;

    [Ignore] public virtual bool ForcefullyFoolish { get; set; } = false;

    [Ignore] public virtual bool Fooled { get; set; } = false;

    public virtual string PluginVer { get; set; } = string.Empty;

    // public virtual void OnReload() { }

    // public virtual void Changed() { }

    // public virtual void CopyFrom(CustomSaberConfig other) { }
}
