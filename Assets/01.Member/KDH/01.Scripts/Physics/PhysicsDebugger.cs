using _01.Member.KMJ._02.Scripts._01.Player;
using UnityEngine;

namespace _01.Member.KDH._01.Scripts.Physics
{
    [RequireComponent(typeof(CharacterMovement))]
    public class PhysicsDebugger : MonoBehaviour
    {
        [Header("Display Settings")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool showVelocityArrow = true;
        [SerializeField] private bool showGroundNormal = true;
        [SerializeField] private KeyCode toggleDebugKey = KeyCode.F3;
        
        [Header("GUI Style")]
        [SerializeField] private int fontSize = 14;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.5f);
        
        private CharacterMovement _movement;
        private GUIStyle _style;
        private GUIStyle _boxStyle;
        private bool _debugEnabled = true;
        
        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
        }
        
        private void Start()
        {
            _style = new GUIStyle();
            _style.fontSize = fontSize;
            _style.normal.textColor = textColor;
            _style.alignment = TextAnchor.UpperLeft;
            
            _boxStyle = new GUIStyle();
            _boxStyle.normal.background = MakeTex(2, 2, backgroundColor);
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(toggleDebugKey))
            {
                _debugEnabled = !_debugEnabled;
            }
        }
        
        private void OnGUI()
        {
            if (!showDebugInfo || !_debugEnabled) return;
            
            int yOffset = 10;
            int lineHeight = fontSize + 5;
            int boxWidth = 300;
            int boxHeight = 250;
            
            GUI.Box(new Rect(5, 5, boxWidth, boxHeight), "", _boxStyle);
            
            DrawLabel($"=== Physics Debug ===", ref yOffset, lineHeight);
            DrawLabel($"Speed: {_movement.currentSpeed:F2} u/s", ref yOffset, lineHeight);
            DrawLabel($"Top Speed: {_movement.topSpeed:F2} u/s", ref yOffset, lineHeight);
            DrawLabel($"Grounded: {_movement.CheckGroundDetected()}", ref yOffset, lineHeight);
            DrawLabel($"Jump Count: {_movement._jumpCnt}/{_movement.maxJumpCnt}", ref yOffset, lineHeight);
            DrawLabel($"Move Speed: {_movement.moveSpeed:F2}", ref yOffset, lineHeight);
            DrawLabel($"Max Speed: {_movement.maxmoveSpeed:F2}", ref yOffset, lineHeight);
            
            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                DrawLabel($"Velocity: ({rb.linearVelocity.x:F1}, {rb.linearVelocity.y:F1}, {rb.linearVelocity.z:F1})", ref yOffset, lineHeight);
                DrawLabel($"Y Velocity: {rb.linearVelocity.y:F2}", ref yOffset, lineHeight);
                
                Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                DrawLabel($"Horizontal Speed: {horizontalVel.magnitude:F2}", ref yOffset, lineHeight);
            }
            
            DrawLabel($"\nControls:", ref yOffset, lineHeight);
            DrawLabel($"\n[{toggleDebugKey}] Toggle Debug", ref yOffset, lineHeight);
        }
        
        private void DrawLabel(string text, ref int yOffset, int lineHeight)
        {
            GUI.Label(new Rect(10, yOffset, 400, lineHeight), text, _style);
            yOffset += lineHeight;
        }
        
        private void OnDrawGizmos()
        {
            if (!_debugEnabled || _movement == null) return;
            
            var rb = GetComponent<Rigidbody>();
            if (rb == null) return;
            
            // Velocity arrow (horizontal)
            if (showVelocityArrow)
            {
                Gizmos.color = Color.yellow;
                Vector3 vel = rb.linearVelocity;
                vel.y = 0; // Only horizontal
                if (vel.magnitude > 0.1f)
                {
                    Gizmos.DrawLine(transform.position, transform.position + vel.normalized * 2f);
                    Gizmos.DrawSphere(transform.position + vel.normalized * 2f, 0.1f);
                }
            }
            
            // Ground check
            if (showGroundNormal)
            {
                Gizmos.color = _movement.CheckGroundDetected() ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}