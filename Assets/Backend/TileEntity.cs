using System;
using System.Collections;
using System.Collections.Generic;
using Unisave;
using Unisave.Entities;
using Unisave.Facades;

public class TileEntity : Entity
{
    /// <summary>
    /// Replace this with your own entity attributes
    /// </summary>
    public EntityReference<PlayerEntity> owner;
    public short[] x;
    public short[] y;
    public short[] type;


}
