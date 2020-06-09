using UnityEngine;
using System;
using System.Collections.Generic;

public class GameObjectData
{
    string obj_name;

    public string OBJ_Name
    {
        get { return obj_name; }
    }

    public GameObjectData()
    {
    }

    public GameObjectData(string p_name)
    {
        obj_name = p_name;

    }

    public void Change(string p_name)
    {
        obj_name = p_name;
    }

    public bool IsEqual(string p_name)
    {
        if (string.Equals(obj_name, p_name) == false)
        {
            return false;
        }
        return true;
    }

}
/// <summary>
/// 部件数据
/// </summary>
public class ActorElementData
{
    string mMeshName;
    string mMatName;
    //Color[] mColors;

    public string mesh
    {
        get { return mMeshName; }
    }

    public string material
    {
        get { return mMatName; }
    }

    public ActorElementData()
    {
    }

    public ActorElementData(string meshName, string matName)
    {
        mMeshName = meshName;
        mMatName = matName;

    }

    public void Change(string meshName, string matName)
    {
        mMeshName = meshName;
        mMatName = matName;

    }

    public bool IsEqual(string meshName, string matName)
    {
        if (string.Equals(meshName, mMeshName) == false)
        {
            return false;
        }

        if (string.Equals(matName, mMatName) == false)
        {
            return false;
        }
        return true;
    }

}


