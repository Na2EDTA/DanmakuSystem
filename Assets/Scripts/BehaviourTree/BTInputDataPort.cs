using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTInputDataPort
{
    public string fieldName;
    public System.Type valueType;
    public BTElement element;  //所属节点
    public List<BTOutputDataPort> sources; //数据来源端口
}
