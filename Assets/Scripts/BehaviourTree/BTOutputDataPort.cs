using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTOutputDataPort
{
    public string fieldName;
    public System.Type valueType;
    public BTElement element;  //所属节点
    public List<BTInputDataPort> destinations; //数据输出目标端口
}
