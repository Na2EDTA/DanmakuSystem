using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTOutputDataPort
{
    public string fieldName;
    public System.Type valueType;
    public BTElement element;  //�����ڵ�
    public List<BTInputDataPort> destinations; //�������Ŀ��˿�
}
