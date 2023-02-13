using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WorldmapMainMenu : MonoBehaviour
{
    public Transform[] levelTransforms;
    [SerializeField]
    private Transform[] directTransforms;
    [SerializeField]
    private Transform[] afterDirectTransforms;

    public void FlashPosition(int level)
    {
        if (levelTransforms != null)
        {
            int length = levelTransforms.Length;
            for (int i = 0; i < length; i++)
            {
                levelTransforms[i].gameObject.SetActive(false);
            }
            levelTransforms[level].gameObject.SetActive(true);
        }
    }
    public Vector3 SetRotationPlayer(int level)
    {
        int y_rotation = 0;
        if (directTransforms != null||afterDirectTransforms!=null)
        {
            int length = afterDirectTransforms.Length;
            if (level >= length)
            {
                level = length-1;
            }
            float X_current = directTransforms[level].position.x;
            float X_after = afterDirectTransforms[level].position.x;
            float Z_current = directTransforms[level].position.z;
            float Z_after = afterDirectTransforms[level].position.z;
            if(X_after - X_current >= 0.1f)
            {
                y_rotation = 90;
            }
            if (X_after - X_current <= -0.1f)
            {
                y_rotation = -90;
            }
            if (Z_after - Z_current >= 0.1f)
            {
                y_rotation = 0;
            }
            if (Z_after - Z_current <= -0.1f)
            {
                y_rotation = 180;
            }
        }
        Vector3 vtRotation = new Vector3(0, y_rotation, 0);
        return vtRotation;
    }
}
