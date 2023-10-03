using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRuntime : MonoBehaviour
{
    public BTTree tree;

    private void Start()
    {
        tree = tree.Clone();
        tree.runtime = this;
    }

    void Update()
    {
        tree.Update();
    }
}
