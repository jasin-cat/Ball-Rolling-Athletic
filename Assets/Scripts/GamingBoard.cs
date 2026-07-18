using UnityEngine;

public class GamingBoard : MonoBehaviour
{
    [SerializeField] Transform _gamingBoardTansform;

    private void Start()
    {
        transform.rotation*=Quaternion.AngleAxis(30, Vector3.up);
    }

    private void Update()
    {
        
    }
}
