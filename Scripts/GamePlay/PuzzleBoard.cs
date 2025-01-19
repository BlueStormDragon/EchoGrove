using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    #region Variables 
    //block types for the 2d array block puzzle
    private enum board { empty, hole, block, kart, terrainBlock, invalid }

    [SerializeField] int boardX, boardY;

    board[,] blockBoard;

    private float squareDistance = 2;
    private List<Vector2> dfd = new List<Vector2>();

    //for the spatial partitioning pattern
    [SerializeField] private List<Vector2> blockCoords = new List<Vector2>(), 
        invalidCoords = new List<Vector2>(), holeCoords = new List<Vector2>(), terrainBlockCoords = new List<Vector2>();

    [SerializeField] private List<GameObject> blocks = new List<GameObject>(), terrainBlocks = new List<GameObject>(), karts = new List<GameObject>();

    [SerializeField] private List<Vector3> kartCoords = new List<Vector3>();

    [SerializeField] private List<GameObject> holeColliders = new List<GameObject>(), terrainObjects = new List<GameObject>();

    [SerializeField] GameObject player;
    private PlayerMovement pMovement;

    private bool moving = false;
    private float pushTime = 2;

    private BlockManager blockManager;
    #endregion

    void Start()
    {
        //Initialize the boards
        blockBoard = new board[boardX, boardY];

        //fill the puzzles
        FillPuzzleBoards();

        blockManager = BlockManager.Instance;
        pMovement = player.GetComponent<PlayerMovement>();
    }

    private void FillPuzzleBoards()
    {
        for(int i = 0; i < boardX; i++)
        {
            for(int j = 0; j < boardY; j++)
            {
                blockBoard[i, j] = board.empty;
            }
        }

        //places all blocks
        for (int i = 0; i < blockCoords.Count; i++)
        {
            blockBoard[(int)blockCoords[i].x, (int)blockCoords[i].y] = board.block;
        }
        //places all invalids
        for (int i = 0; i < invalidCoords.Count; i++)
        {
            blockBoard[(int)invalidCoords[i].x, (int)invalidCoords[i].y] = board.invalid;
        }
        //places all holes
        for (int i = 0; i < holeCoords.Count; i++)
        {
            blockBoard[(int)holeCoords[i].x, (int)holeCoords[i].y] = board.hole;
        }
        //places all terrainblocks
        for (int i = 0; i < terrainBlockCoords.Count; i++)
        {
            blockBoard[(int)terrainBlockCoords[i].x, (int)terrainBlockCoords[i].y] = board.terrainBlock;
        }
        //places all karts
        for (int i = 0; i < kartCoords.Count; i++)
        {
            blockBoard[(int)kartCoords[i].x, (int)kartCoords[i].y] = board.kart;
        }
    }

    public bool ValidBlockMove(Vector2 startPos, Vector2 endPos, bool backing, Direction dirToMove, GameObject objectToMove)
    {
        //if endpos would be outside the bounds of the array
        if (endPos.x < 0 || endPos.y < 0 || endPos.x >= boardX || endPos.y >= boardY)
        {
            PlayBackingFailSFX();
            return false;
        }

        //check if the player is pushing a kart in its axis, if not return false
        if(blockBoard[(int)startPos.x, (int)startPos.y] == board.kart)
        {
            
            if (kartCoords[karts.IndexOf(objectToMove)].z == 1)
            {
                if (dirToMove == Direction.up || dirToMove == Direction.down)
                    return false;
            }       
            else
            {
                if (dirToMove == Direction.left || dirToMove == Direction.right)
                    return false;
            }      
        }
        //then we check if the end position is either empty or a hole
        if (blockBoard[(int)endPos.x, (int)endPos.y] < board.block)
        {
            //if the player is pulling, check if the player would be backed into something
            if(backing)
            {
                if(dirToMove == Direction.up || dirToMove == Direction.down)
                {
                    switch (dirToMove)
                    {
                        case Direction.up:

                        //checks if player isn't backing out of puzzle array, if so fail the backing
                        if ((int)endPos.y > 0 && (int)endPos.y < boardY - 1)
                        {
                            if (blockBoard[(int)endPos.x, (int)endPos.y + 1] == board.empty)
                                return true;
                        }
                        StartCoroutine(BackingFail(objectToMove, new Vector2(0, 1)));
                        return false;

                    case Direction.down:
                        
                        if ((int)endPos.y > 0 && (int)endPos.y < boardY - 1)
                        {
                            if (blockBoard[(int)endPos.x, (int)endPos.y - 1] == board.empty)
                                return true;
                        }

                        StartCoroutine(BackingFail(objectToMove, new Vector2(0, -1)));
                        return false;
                        
                        default: break;
                    }
                    
                }
                else if(dirToMove == Direction.left || dirToMove == Direction.right)
                {
                    
                    switch (dirToMove)
                    {
                        case Direction.right:

                            if ((int)endPos.x > 0 && (int)endPos.x < boardX - 1)
                            {
                                if (blockBoard[(int)endPos.x + 1, (int)endPos.y] == board.empty)
                                return true;
                            }
                        

                            StartCoroutine(BackingFail(objectToMove, new Vector2(1, 0)));
                            return false;

                        case Direction.left:
                            
                            if ((int)endPos.x > 0 && (int)endPos.x < boardX - 1)
                            {
                                if (blockBoard[(int)endPos.x - 1, (int)endPos.y] == board.empty)
                                return true;
                            }
                            StartCoroutine(BackingFail(objectToMove, new Vector2(-1, 0)));
                            return false;
                            
                        default: break;
                    }
                    
                }
            }

            //if player tries to push a terrain block we return false, terrainblocks can only be pulled
            else if (blockBoard[(int)startPos.x, (int)startPos.y] == board.terrainBlock)
                return false; 
            
            return true;
        }
        return false;
    }

    //move the blocks in the board array
    public void UpdateObjectLists(Vector2 startPos, Vector2 endPos, GameObject objectToMove, bool backing)
    {
        //karts move differently 
        if(blockBoard[(int)startPos.x, (int)startPos.y] == board.kart)
        {
            if(!backing)
            {
                pMovement.ChangeAnimation("Pushing");

                StartCoroutine(CalculateKartMovement(startPos, endPos, objectToMove));
                return;
            }
        }

        switch(blockBoard[(int)startPos.x, (int)startPos.y])
        {
            case board.block:
                //if the player pushes the block over a hole
                if (blockBoard[(int)endPos.x, (int)endPos.y] == board.hole)
                {
                    //make the spaces empty in the array
                    blockBoard[(int)endPos.x, (int)endPos.y] = board.empty;
                    blockBoard[(int)startPos.x, (int)startPos.y] = board.empty;

                    //correct the spatial partition lists
                    int index = blocks.IndexOf(objectToMove);
                    blocks.RemoveAt(index);
                    blockCoords.RemoveAt(index);

                    int holeIndex = holeCoords.IndexOf(new Vector2(endPos.x, endPos.y));
                    holeCoords.RemoveAt(holeIndex);

                    Destroy(holeColliders[holeIndex]);
                    holeColliders.RemoveAt(holeIndex);
                    
                    if(backing)
                        pMovement.ChangeAnimation("Pulling");
                    else
                        pMovement.ChangeAnimation("Pushing");

                    //Move the blocks in the scene
                    StartCoroutine(BlockMove(objectToMove, endPos - startPos, true, backing, false, -1));
                }
                else
                {
                    //make the move on the board
                    board temp = blockBoard[(int)endPos.x, (int)endPos.y];
                    blockBoard[(int)endPos.x, (int)endPos.y] = blockBoard[(int)startPos.x, (int)startPos.y];
                    blockBoard[(int)startPos.x, (int)startPos.y] = temp;

                    //correct the spatial partition lists
                    int index = blocks.IndexOf(objectToMove);
                    blockCoords[index] = new Vector2((int)endPos.x, (int)endPos.y);

                    if (backing)
                        pMovement.ChangeAnimation("Pulling");
                    else
                        pMovement.ChangeAnimation("Pushing");

                    //Move the blocks in the scene
                    StartCoroutine(BlockMove(objectToMove, endPos - startPos, false, backing, false, -1));
                }
                break;
            case board.terrainBlock:
                //make the move on the board
                blockBoard[(int)endPos.x, (int)endPos.y] = board.block;
                blockBoard[(int)startPos.x, (int)startPos.y] = board.invalid;

                //remove the terrain block from the terrainBlock list 
                int listIndex = terrainBlocks.IndexOf(objectToMove);
                terrainBlockCoords.RemoveAt(listIndex);
                terrainBlocks.RemoveAt(listIndex);

                //and move it to the block list
                blocks.Add(objectToMove);
                blockCoords.Add(new Vector2((int)endPos.x, (int)endPos.y));

                if (backing)
                    pMovement.ChangeAnimation("Pulling");

                //Move the blocks in the scene
                StartCoroutine(BlockMove(objectToMove, endPos - startPos, false, backing, true, listIndex));
                break;
            case board.kart:
                //make the move on the board
                board tempBoard = blockBoard[(int)endPos.x, (int)endPos.y];
                blockBoard[(int)endPos.x, (int)endPos.y] = blockBoard[(int)startPos.x, (int)startPos.y];
                blockBoard[(int)startPos.x, (int)startPos.y] = tempBoard;

                //correct the spatial partition lists
                int tempIndex = karts.IndexOf(objectToMove);
                kartCoords[tempIndex] = new Vector3((int)endPos.x, (int)endPos.y, kartCoords[tempIndex].z);

                if (backing)
                    pMovement.ChangeAnimation("Pulling");
                else
                    pMovement.ChangeAnimation("Pushing");

                //Move the blocks in the scene
                StartCoroutine(BlockMove(objectToMove, endPos - startPos, false, backing, false, -1));
                break;
        }
    }

    //move the kart in the board array
    private IEnumerator CalculateKartMovement(Vector2 startPos, Vector2 endPos, GameObject objectToMove)
    {
        //make the player drop his grip
        blockManager.HandsOffBlock();
        
        Vector2 dirToMove = endPos - startPos;
        Vector2 currentPos = startPos;
        Vector2 goalPos = new Vector2(0,0);

        //we check every space in a straight line until we hit a hole, invalid space or outiside the array
        bool looping = true;
        bool hole = false;
        
        while (looping)
        {
            //checks if the kart has reached the end of the board
            if ((int)currentPos.x + dirToMove.x >= boardX ||
                (int)currentPos.y + dirToMove.y >= boardY ||
                (int)currentPos.x + dirToMove.x < 0 ||
                (int)currentPos.y + dirToMove.y < 0)
            {
                goalPos = currentPos;
                looping = false;
                continue;
            }
            //checks if the kart has reached an obstacle
            else if (blockBoard[(int)currentPos.x + (int)dirToMove.x, (int)currentPos.y + (int)dirToMove.y] > board.hole)
            {
                goalPos = currentPos;
                looping = false;
                continue;
            }
            else if (blockBoard[(int)currentPos.x + (int)dirToMove.x, (int)currentPos.y + (int)dirToMove.y] == board.hole)
            {
                //if next space is a hole: before we exit the loop we want 
                //to make sure the goalpos is the space with the hole
                currentPos += dirToMove;

                goalPos = currentPos;
                hole = true;
                looping = false;
                continue;
            }
            currentPos += dirToMove;
        }
            
        //move the kart in the scene
        looping = true;
        currentPos = startPos;

        blockManager.blockPushingAS.PlayOneShot(blockManager.kartSFX);
        while (looping)
        {
            //early return
            if (currentPos == goalPos)
                break;

            yield return StartCoroutine(KartMove(objectToMove, endPos - startPos, false));
            currentPos += dirToMove;

            yield return null;
        }

        blockManager.blockPushingAS.Stop();

        if (hole)
        {
            yield return StartCoroutine(KartMove(objectToMove, new Vector2(0, 0), true));

            //remove the hole from holecoords
            int holeIndex = holeCoords.IndexOf(new Vector2((int)goalPos.x, (int)goalPos.y));
            holeCoords.RemoveAt(holeIndex);
            
            Destroy(holeColliders[holeIndex]);
            holeColliders.RemoveAt(holeIndex);

            //make the spaces empty in the array
            blockBoard[(int)goalPos.x, (int)goalPos.y] = board.empty;
            blockBoard[(int)startPos.x, (int)startPos.y] = board.empty;

            //correct the spatial partition lists
            int listIndex = karts.IndexOf(objectToMove);
            karts.RemoveAt(listIndex);
            kartCoords.RemoveAt(listIndex);
        }
        else
        {
            //make the move on the board
            board temp = blockBoard[(int)goalPos.x, (int)goalPos.y];
            blockBoard[(int)goalPos.x, (int)goalPos.y] = blockBoard[(int)startPos.x, (int)startPos.y];
            blockBoard[(int)startPos.x, (int)startPos.y] = temp;

            //correct the spatial partition lists
            int listIndex = karts.IndexOf(objectToMove);
            kartCoords[listIndex] = new Vector3((int)goalPos.x, (int)goalPos.y, kartCoords[listIndex].z);
        }
    }

    private IEnumerator BackingFail(GameObject objectToMove, Vector2 direction)
    {
        PlayBackingFailSFX();
        moving = true;
        Vector3 startPos = objectToMove.transform.position;
        GameObject temp = new GameObject();
        GameObject parent = Instantiate(temp, objectToMove.transform.position,Quaternion.identity);

        Vector3 targetPos = parent.transform.position + new Vector3(direction.x * (squareDistance / 2), 0, direction.y * (squareDistance / 2));

        objectToMove.transform.SetParent(parent.transform);
        player.transform.SetParent(parent.transform);

        Vector3 playerPos = player.transform.position;
        if(!isOffset)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, objectToMove.transform.position, 0.2f);
            isOffset = true;
        }

        pMovement.ChangeAnimation("Pulling");

        while (objectToMove.transform.position != targetPos)
        {
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, targetPos, pushTime * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        targetPos = startPos;
        while (objectToMove.transform.position != targetPos)
        {
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, targetPos, pushTime * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        objectToMove.transform.parent = null;
        player.transform.parent = null;
        Destroy(temp);
        Destroy(parent);

        moving = false;
    }

    private void PlayBackingFailSFX()
    {
        blockManager.PlaybackingFailSFX();
    }
    private void PlayRockFallSFX()
    {
        blockManager.PlayFallingRockSFX();
    }
    private void PlayBlockFallSFX()
    {
        blockManager.PlayFallingBlockSFX();
    }

    //move the karts in the scene
    private IEnumerator KartMove(GameObject objectToMove, Vector2 direction, bool hole)
    {
        moving = true;
        Vector3 targetPos = objectToMove.transform.position + new Vector3(direction.x * squareDistance, 0, direction.y * squareDistance);
        while (objectToMove.transform.position != targetPos)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, targetPos, pushTime * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        if (hole)
        {
            yield return StartCoroutine(BlockFall(objectToMove, false));
        }

        blockManager.PlayerLetGO();

        moving = false;
    }

    //true if the player has been moved because of the backing animation
    private bool isOffset = false;

    //move the blocks in the scene
    private IEnumerator BlockMove(GameObject objectToMove, Vector2 direction, bool hole, bool backing, bool terrainBlock, int objectIndex)
    {
        moving = true;
        //the pc backing animation moves the character too far back so we compensate and move the player 
        //towards the block if the player is backing. If the player backs twice in a row we dont want to
        //move the pc twice. And when the player pushed if the pc has been moved we have to move the pc 
        //to where he would've been standing.
        if(!backing && isOffset)
        {
            Vector3 diff = objectToMove.transform.position - player.transform.position;
            Vector3 targetPos2 = player.transform.position - (diff / 1.9f);
            float distance = Vector3.Distance(player.transform.position, new Vector3(targetPos2.x, player.transform.position.y, targetPos2.z));
            isOffset = false;

            while (player.transform.position != new Vector3(targetPos2.x, player.transform.position.y, targetPos2.z))
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position,
                    new Vector3(targetPos2.x, player.transform.position.y, targetPos2.z), distance/3);
            }
        }

        if (backing && !isOffset)
        {
            isOffset = true;
            Vector3 targetPos3 = Vector3.Lerp(player.transform.position, objectToMove.transform.position, 0.35f);  
            float distance = Vector3.Distance(player.transform.position, targetPos3);

            while (player.transform.position != new Vector3(targetPos3.x, player.transform.position.y, targetPos3.z))
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position,
                    new Vector3(targetPos3.x, player.transform.position.y, targetPos3.z), distance / 3);
            }
        }


        GameObject temp = new GameObject();
        GameObject parent = Instantiate(temp, objectToMove.transform.position,Quaternion.identity);

        Vector3 targetPos = parent.transform.position + new Vector3(direction.x * squareDistance, 0, direction.y * squareDistance);
       
        objectToMove.transform.SetParent(parent.transform);
        player.transform.SetParent(parent.transform);

        blockManager.blockPushingAS.Play();
        
        //if we're pulling a terrainblock we want to play the falling animation in the middle of the pull
        if (terrainBlock)
        {
            float midPoint = Vector3.Distance(targetPos, parent.transform.position) / 2;
            bool hasFallen = false;

            while (parent.transform.position != targetPos)
            {
                parent.transform.position = Vector3.MoveTowards(parent.transform.position, targetPos, pushTime * Time.deltaTime);

                if (!hasFallen && Vector3.Distance(parent.transform.position, targetPos) < midPoint)
                {
                    hasFallen = true;
                    terrainObjects[objectIndex].GetComponent<Animator>().Play("Fall");
                    yield return new WaitForSeconds(0.2f);
                    PlayRockFallSFX();
                    terrainObjects.RemoveAt(objectIndex);
                }

                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (parent.transform.position != targetPos)
            {
                parent.transform.position = Vector3.MoveTowards(parent.transform.position, targetPos, pushTime * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
        
        blockManager.blockPushingAS.Stop();

        objectToMove.transform.parent = null;
        player.transform.parent = null;
        Destroy(temp);
        Destroy(parent);

        if (hole)
        {
            yield return StartCoroutine(BlockFall(objectToMove, true));
        }
        moving = false;
    } 

    private IEnumerator BlockFall(GameObject objectToMove, bool isTerrain)
    {
        if(!isTerrain)
            blockManager.HandsOffBlock();

        Vector3 startingPos = objectToMove.transform.position;
        Vector3 targetPos;

        //we want to check if the object is a kart because karts fall a shorter distance
        if (GetList(objectToMove) == karts)
            targetPos = startingPos - new Vector3(0, 1.5f, 0f);
        else
            targetPos = startingPos - new Vector3(0, 2f, 0f);


        PlayBlockFallSFX();

        while (objectToMove.transform.position != targetPos)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position,
            targetPos, pushTime * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    //get the board position of the block
    public Vector3 BlockPosition(GameObject block)
    {
        if (terrainBlocks.Contains(block))
            return terrainBlockCoords[terrainBlocks.IndexOf(block)];
        else if (karts.Contains(block))
                return kartCoords[karts.IndexOf(block)];
        return blockCoords[blocks.IndexOf(block)];
    }

    public bool GetMoving()
    {
        return moving;
    }

    //returns true if the given side of the block is empty
    public bool GetObjectNextToBlock(GameObject block, Direction lookDir)
    {
        Vector2 blockPos = Vector2.zero;
        int index = 0;
        
        if (blocks.Contains(block))
        {
            index = blocks.IndexOf(block);
            blockPos = blockCoords[index];

        }
        else if(karts.Contains(block))
        {
            index = karts.IndexOf(block);
            blockPos = kartCoords[index];
        }
        else
        {
            index = terrainBlocks.IndexOf(block);
            blockPos = terrainBlockCoords[index];
        }

        switch (lookDir)
        {
            case Direction.down:
                if (blockPos.y + 1 >= boardY)
                {
                    if (IsPlayerClippingIntoObject())
                        return false;
                    else
                        return true;
                }

                if (blockBoard[(int)blockPos.x, (int)blockPos.y + 1] == board.empty)
                    return true;
                break;
            case Direction.left:
                if(blockPos.x + 1 >= boardX)
                {
                    if (IsPlayerClippingIntoObject())
                        return false;
                    else
                        return true;
                }

                if (blockBoard[(int)blockPos.x + 1, (int)blockPos.y] == board.empty)
                {
                    return true;
                }
                break;
            case Direction.up:
                if (blockPos.y - 1 < 0)
                {
                    if (IsPlayerClippingIntoObject())
                        return false;
                    else
                        return true;
                }

                if (blockBoard[(int)blockPos.x, (int)blockPos.y - 1] == board.empty)
                    return true;
                break;
            case Direction.right:
                if(blockPos.x - 1 < 0)
                {
                    if (IsPlayerClippingIntoObject())
                        return false;
                    else
                        return true;
                }

                if (blockBoard[(int)blockPos.x - 1, (int)blockPos.y] == board.empty)
                    return true;
                break;
        }
        return false;
    }

    private bool IsPlayerClippingIntoObject()
    {
        Collider[] maxColliders = new Collider[5];
        int collidersClipped = Physics.OverlapSphereNonAlloc(blockManager.GetBlockFacePos(), 0.2f, maxColliders);

        for (int i = 0; i < collidersClipped; i++)
        {
            if (maxColliders[i].gameObject == player || maxColliders[i].gameObject.name == "Hands")
            {
                collidersClipped--;
                i--;
            }
        }

        if (collidersClipped > 0)
            return true;
            
        else
            return false;
    }

    //returns the list the block is in
    public List<GameObject> GetList(GameObject block)
    {
        //checks the terrainblock list first because it's shorter most of the time
        if(terrainBlocks.Contains(block))
            return terrainBlocks;
        else if(karts.Contains(block))
            return karts;
        else
            return blocks;
    }

    public void TurnOffOffset()
    {
        isOffset = false;
    }
}