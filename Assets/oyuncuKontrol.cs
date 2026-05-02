using UnityEngine;
using UnityEngine.InputSystem;

public class oyuncuKontrol : MonoBehaviour
{
    public float speed=0.0f;
    public Rigidbody2D rigidbody2D;
    private Animator animator;
    private Vector3 charPos;
    [SerializeField] GameObject _camera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D=GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            speed=1.0f;
            Debug.Log(message: "hiz 1.0");
                    }
        else
        {
            speed=0.0f;
            Debug.Log(message: "hiz 0.0");
        }
        animator.SetFloat(name:"speed",speed);
        //rigidbody2D.linearVelocity=new Vector2(x:speed,y:0f);
        Vector3 charPos = transform.position;
        charPos=new Vector3(x:charPos.x+(speed*Time.deltaTime),charPos.y,charPos.z);
        transform.position=charPos;
    }
    private void LateUpdate()
    {
        _camera.transform.position=new Vector3(charPos.x,charPos.y,charPos.z-1.0f);
    }
}
