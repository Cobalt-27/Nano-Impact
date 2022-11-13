using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nano
{
    public class CameraScript : MonoBehaviour
    {
        public bool Enable = true;
        public Vector3 target;
        [SerializeField]
        private float dragSpeed = 0.02f;
        [SerializeField]
        private float scaleSpeed = 4f;
        private Vector3 dragOrigin;

        [SerializeField]
        private float minHeight = 3;
        [SerializeField]
        private float maxHeight = 12;
        [SerializeField]
        private float smoothing = 0.5f;
        [SerializeField]
        private float rotateSpeed=1f;
        // Start is called before the first frame update
        void Start()
        {
            target = gameObject.transform.position;
        }
        void LateUpdate()
        {
            if (Enable)
            {
                HandleMove();
                HandleScale();
                SmoothTranslate();
                HandleRotate();
            }
        }
        private void HandleRotate(){
            if(Input.GetKey(KeyCode.Q)){
                gameObject.transform.Rotate(Vector3.down*rotateSpeed,Space.Self);
            }
            if(Input.GetKey(KeyCode.E)){
                gameObject.transform.Rotate(-Vector3.down*rotateSpeed,Space.Self);
            }
        }
        private void SmoothTranslate()
        {
            if (transform.position != target)
            {
                transform.position = Vector3.Lerp
                (transform.position,
                target, smoothing);
            }
        }
        private void HandleMove()
        {
            if (!Input.GetMouseButton(2))
            {
                return;
            }
            if (Input.GetMouseButtonDown(2))
            {
                dragOrigin = Input.mousePosition;
                return;
            }
            var pos = Input.mousePosition - dragOrigin;
            
            Vector3 move = new Vector3(pos.y * dragSpeed, 0, -pos.x * dragSpeed);
            move=gameObject.transform.TransformVector(move);
            target += move;
            dragOrigin = Input.mousePosition;
        }

        private void HandleScale()
        {
            target.y -= scaleSpeed * Input.GetAxis("Mouse ScrollWheel");
            target.y = Mathf.Min(target.y, maxHeight);
            target.y = Mathf.Max(target.y, minHeight);
        }
    }
}