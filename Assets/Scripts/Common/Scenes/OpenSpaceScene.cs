using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xverse.Scene;

public class OpenSpaceScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Xverse.Scene.Scene.OpenSpace;
    }
    public override void Clear()
    {

    }
}
