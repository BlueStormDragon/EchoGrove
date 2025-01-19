using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { up, right, down, left, invalid}
public class BlockManager : MonoBehaviour
{
    private KeyCode moveButton = KeyCode.Mouse1; // trigger
    private KeyCode moveButton1 = KeyCode.Joystick1Button0; // trigger
   
    [SerializeField] GameObject player;
    private PlayerMovement pMovement;

    [SerializeField] List<GameObject> listOfPuzzles = new List<GameObject>();
    private PuzzleBoard puzzle;

    [SerializeField] public AudioSource blockPushingAS;
    [SerializeField] public AudioClip kartSFX;

    public static BlockManager Instance;
    private DialogManager diaManager;

    private ControllerCheck controller = new ControllerCheck();

    [SerializeField] private Hands hands;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        pMovement = player.GetComponent<PlayerMovement>();
        diaManager = player.GetComponent<DialogManager>();

        StartCoroutine(HoldBlock());
    }

    private GameObject heldObject;
    Direction lookingDir;
    private Vector3 blockFacePos;
    private bool playerLetGo = false;

    private IEnumerator HoldBlock()
    {
        while(true)
        {
            if ((Input.GetKeyDown(moveButton) || Input.GetKeyDown(moveButton1)) && heldObject != null && !diaManager.InDialogue())
            {
                //gravity causes unintended consequences
                pMovement.TurnOffGravity();

                //Set which puzzle the block belongs to
                FindPuzzle(heldObject);

                pMovement.StopMovement();

                //we only want to know wether the player is looking up/down or left/right 
                heldObject = hands.NearestBlock(player);
                lookingDir = PlayerDirection(heldObject);

                //can the player stand on that side of the block?
                if (puzzle.GetObjectNextToBlock(heldObject, lookingDir))
                    yield return StartCoroutine(MovePlayerToFaceBlock(blockFacePos, heldObject.transform.position));
                else
                    playerLetGo = true;
            }

            //while holding the block
            while ((Input.GetKey(moveButton) || Input.GetKey(moveButton1)) && heldObject != null)
            {
                switch (lookingDir)
                {
                    case Direction.up:
                        if(controller.CheckForController())
                        {
                            if (!puzzle.GetMoving() && (Input.GetAxis("Vertical") > 0.5f || Input.GetAxis("Horizontal") < -0.5f))
                            {
                                //get the blocks position on the board
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                //check if the position above it is a legal move
                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x, startPos.y + 1), false, Direction.up, heldObject))
                                {
                                    //if it is make the move
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x, startPos.y + 1), heldObject, false);
                                }
                            }
                            else if (!puzzle.GetMoving() && (Input.GetAxis("Vertical") < -0.5f || Input.GetAxis("Horizontal") > 0.5f))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x, startPos.y - 1), true, Direction.down, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x, startPos.y - 1), heldObject, true);
                            }
                        }
                        else
                        {
                            if (!puzzle.GetMoving() && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x, startPos.y + 1), false, Direction.up, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x, startPos.y + 1), heldObject, false);
                            }
                            else if (!puzzle.GetMoving() && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x, startPos.y - 1), true, Direction.down, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x, startPos.y - 1), heldObject, true);
                            }
                        }
                        break;
                    case Direction.right:
                        if (controller.CheckForController())
                        {
                            if (!puzzle.GetMoving() && (Input.GetAxis("Horizontal") > 0.5f || Input.GetAxis("Vertical") > 0.5f))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x + 1, startPos.y), false, Direction.right, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x + 1, startPos.y), heldObject, false);
                            }
                            else if (!puzzle.GetMoving() && (Input.GetAxis("Horizontal") < -0.5f || Input.GetAxis("Vertical") < -0.5f))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x - 1, startPos.y), true, Direction.left, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x - 1, startPos.y), heldObject, true);
                            }
                        }
                        else
                        {
                            if (!puzzle.GetMoving() && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W)))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x + 1, startPos.y), false, Direction.right, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x + 1, startPos.y), heldObject, false);
                            }
                            else if (!puzzle.GetMoving() && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S)))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x - 1, startPos.y), true, Direction.left, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x - 1, startPos.y), heldObject, true);
                            }
                        }
                        break;
                    case Direction.down:
                        if (controller.CheckForController())
                        {
                            if (!puzzle.GetMoving() && (Input.GetAxis("Vertical") > 0.5f || Input.GetAxis("Horizontal") < -0.5f))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x, startPos.y + 1), true, Direction.up, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x, startPos.y + 1), heldObject, true);
                            }
                            else if (!puzzle.GetMoving() && (Input.GetAxis("Vertical") < -0.5f || Input.GetAxis("Horizontal") > 0.5f))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x, startPos.y - 1), false, Direction.down, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x, startPos.y - 1), heldObject, false);
                            }
                        }
                        else
                        {
                            if (!puzzle.GetMoving() && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x, startPos.y + 1), true, Direction.up, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x, startPos.y + 1), heldObject, true);
                            }
                            else if (!puzzle.GetMoving() && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x, startPos.y - 1), false, Direction.down, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x, startPos.y - 1), heldObject, false);
                            }
                        }
                        break;
                    case Direction.left:
                        if (controller.CheckForController())
                        {
                            if (!puzzle.GetMoving() && (Input.GetAxis("Horizontal") > 0.5f || Input.GetAxis("Vertical") > 0.5f))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x + 1, startPos.y), true, Direction.right, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x + 1, startPos.y), heldObject, true);
                            }
                            else if (!puzzle.GetMoving() && (Input.GetAxis("Horizontal") < -0.5f || Input.GetAxis("Vertical") < -0.5f))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x - 1, startPos.y), false, Direction.left, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x - 1, startPos.y), heldObject, false);
                            }
                        }
                        else
                        {
                            if (!puzzle.GetMoving() && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W)))
                            {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x + 1, startPos.y), true, Direction.right, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x + 1, startPos.y), heldObject, true);
                            }
                            else if (!puzzle.GetMoving() && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S)))
                                {
                                Vector2 startPos = puzzle.BlockPosition(heldObject);

                                if (puzzle.ValidBlockMove(startPos, new Vector2(startPos.x - 1, startPos.y), false, Direction.left, heldObject))
                                    puzzle.UpdateObjectLists(new Vector2(startPos.x, startPos.y), new Vector2(startPos.x - 1, startPos.y), heldObject, false);
                            }
                        }
                        
                        break;
                }

                yield return null;
            }

            if (((Input.GetKeyUp(moveButton) || Input.GetKeyUp(moveButton1)) && !diaManager.InDialogue() && heldObject != null) || playerLetGo)
            {
                //make sure the player doesn't get to move until the blocks movement is done
                while (puzzle != null)
                {
                    if (!puzzle.GetMoving())
                        break;
                    else
                        yield return null;
                }

                //return the player speed to normal
                pMovement.StartMovement();

                playerLetGo = false;
                pMovement.TurnOnGravity();

                if (puzzle != null)
                    puzzle.TurnOffOffset();
            }

            yield return null;
        }
    }
    
    //the direction the player is looking when pushing block
    private Direction PlayerDirection(GameObject block)
    {
        //create 4 points on each face of the block
        List<Vector3> blockPoints = new List<Vector3>();

        for(int i = 0; i < 4; i++)
        {
            switch(i)
            {
                case 0:
                    //north facing side of the block
                    blockPoints.Add(new Vector3(block.transform.position.x, player.transform.position.y, block.transform.position.z + 1.5f));
                    break;
                case 1:
                    //east facing side of the block
                    blockPoints.Add(new Vector3(block.transform.position.x + 1.5f, player.transform.position.y, block.transform.position.z));
                    break;
                case 2:
                    //south facing side of the block
                    blockPoints.Add(new Vector3(block.transform.position.x, player.transform.position.y, block.transform.position.z - 1.5f));
                    break;
                case 3:
                    //west facing side of the block
                    blockPoints.Add(new Vector3(block.transform.position.x - 1.5f, player.transform.position.y, block.transform.position.z));
                    break;
            }
        }

        //check player position and check which point is closest to the player
        int index = 0;
        float distance = Vector3.Distance(blockPoints[0], player.transform.position);
        for (int i = 1; i < 4; i++)
        {
            if(distance > Vector3.Distance(blockPoints[i], player.transform.position))
            {
                distance = Vector3.Distance(blockPoints[i], player.transform.position);
                index = i;
            }
        }
        
        //return the direction the player is facing
        Direction lookDir = 0;

        switch (index)
        {
            case 0:
                lookDir = Direction.down;
                break;
            case 1:
                lookDir = Direction.left;
                break;
            case 2:
                lookDir = Direction.up;
                break;
            case 3:
                lookDir = Direction.right;
                break;
        }
        blockFacePos = blockPoints[index];
        return lookDir;
    }

    private IEnumerator MovePlayerToFaceBlock(Vector3 targetPos, Vector3 block)
    {
        pMovement.ChangeAnimation("ArmsUP_Push");
        float distanceToMove = Vector3.Distance(player.transform.position, targetPos) / 10;

        while(player.transform.position != targetPos)
        {
            //if player let go during the grab animation
            if(Input.GetKeyUp(moveButton) || Input.GetKeyUp(moveButton1))
                playerLetGo = true;

            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, distanceToMove);
            yield return null;
        }
        player.transform.LookAt(new Vector3(block.x, player.transform.position.y, block.z));
    }

    public void HandsOnBlock(GameObject block)
    {
        heldObject = block;     
    }

    public void HandsOffBlock()
    {
        heldObject = null;
    }

    //figure óut which puzzle the block belongs to
    private void FindPuzzle(GameObject block)
    {
        for (int i = 0; i < listOfPuzzles.Count; i++)
        {
            if (listOfPuzzles[i].GetComponent<PuzzleBoard>().GetList(block).Contains(block))
            {
                puzzle = listOfPuzzles[i].GetComponent<PuzzleBoard>();
                return;
            }
        }
    }

    public void PlayerLetGO()
    {
        playerLetGo = true;
    }

    [SerializeField] private AudioSource backFailSource, blockFallSource, rockFallSource;
    public void PlaybackingFailSFX()
    {
        backFailSource.Play();
    }
    public void PlayFallingRockSFX()
    {
        rockFallSource.Play();
    }
    public void PlayFallingBlockSFX()
    {
        blockFallSource.Play();
    }

    public Vector3 GetBlockFacePos()
    {
        return blockFacePos;
    }
}
