using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block
{
    public bool changed = true;
    public byte value = 1;
    public Block(byte value)
    {
        this.value = value;
    }
}
