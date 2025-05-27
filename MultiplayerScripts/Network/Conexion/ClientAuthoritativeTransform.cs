using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;
public class ClientAuthoritativeTransform : NetworkTransform
{
    // Start is called before the first frame update
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
