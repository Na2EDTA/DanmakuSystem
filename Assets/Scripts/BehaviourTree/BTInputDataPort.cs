using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTInputDataPort
{
    public string fieldName;
    public System.Type valueType;
    public BTElement element;  //�����ڵ�
    public List<BTOutputDataPort> sources; //������Դ�˿�
}
