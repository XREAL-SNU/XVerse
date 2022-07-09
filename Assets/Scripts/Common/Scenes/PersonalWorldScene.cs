using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xverse.Scene;

public class PersonalWorldScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Xverse.Scene.Scene.PersonalWorld;
    }
    public override void Clear()
    {

    }
}
