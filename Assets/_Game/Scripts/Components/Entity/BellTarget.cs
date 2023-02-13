using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellTarget : MonoBehaviour
{
    public void ThuHut(Bell bell)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
        for (int i = 0; i < colliders.Length; i++)
        {
            Enemy enemy = colliders[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector3 VectorDistance = bell.transform.position - enemy.transform.position;
                Vector3 targetDirection = VectorDistance / VectorDistance.magnitude;
                Vector3 targetPosition = new Vector3(Mathf.RoundToInt(bell.transform.position.x - targetDirection.x), enemy.transform.position.y,
                    Mathf.RoundToInt(bell.transform.position.z - targetDirection.z));
                if (Mathf.RoundToInt(targetDirection.x) == 1 )
                {
                    enemy.transform.rotation = Quaternion.Euler(90 * Vector3.up);
                }
                else if (Mathf.RoundToInt(targetDirection.x) == -1)
                {
                    enemy.transform.rotation = Quaternion.Euler(90 * Vector3.down);
                }
                else if (Mathf.RoundToInt(targetDirection.z) == 1)
                {
                    enemy.transform.rotation = Quaternion.Euler(Vector3.zero);
                }
                else if(Mathf.RoundToInt(targetDirection.z) == -1)
                {
                    enemy.transform.rotation = Quaternion.Euler(180 * Vector3.up);
                }
                enemy.moveTimeUnit = 0.05f;
                LeanTween.move(enemy.gameObject, targetPosition, Mathf.Abs(VectorDistance.magnitude)*enemy.moveTimeUnit);
            }
        }
    }
}
