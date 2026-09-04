using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction primaryWeaponAction;
    private InputAction secondaryWeaponAction;

    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public Vector2 MoveVector { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputReleased { get; private set; }

    public bool PrimaryWeaponPerformed { get; private set; }
    public bool SecondaryWeaponPerformed { get; private set; }

    public bool PrimaryWeaponReleased { get; private set; }
    public bool SecondaryWeaponReleased { get; private set; }

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        inputActions.Enable();
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");
        primaryWeaponAction = inputActions.FindAction("Primary Weapon");
        secondaryWeaponAction = inputActions.FindAction("Secondary Weapon");
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            inputActions.Disable();
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        MoveVector = moveAction.ReadValue<Vector2>();
        HorizontalInput = MoveVector.x;
        VerticalInput = MoveVector.y;

        JumpInput = jumpAction.WasPerformedThisFrame();
	JumpInputReleased = jumpAction.WasReleasedThisFrame();

        PrimaryWeaponPerformed = primaryWeaponAction.WasPerformedThisFrame();
        SecondaryWeaponPerformed = secondaryWeaponAction.WasPerformedThisFrame();

        PrimaryWeaponReleased = primaryWeaponAction.WasReleasedThisFrame();
        SecondaryWeaponReleased = secondaryWeaponAction.WasReleasedThisFrame();
    }
}
