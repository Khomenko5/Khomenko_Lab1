using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform blockPrefab;
    [SerializeField] private Transform blockHolder;
    [SerializeField] private TriggerZone triggerZone;
    [SerializeField] private TMP_Text blocksCounterText;
    [SerializeField] private float timeBetweenBlocks = 3f;
    [SerializeField] private float fallThreshold = -5f;

    private Transform currentBlock = null;
    private Rigidbody2D currentRigidbody;
    private Vector2 blockStartPosition = new Vector2(0f, 4f);
    private float blockSpeed = 8f;
    private float blockSpeedIncrement = 0.5f;
    private int blockDirection = 1;
    private float xLimit = 5;

    private float timer;
    private bool isWaitingForNextBlock = false;
    private int blocksInGame = 0;
    private List<Transform> allBlocks = new List<Transform>();
    private HashSet<Transform> countedBlocks = new HashSet<Transform>();

    void Start()
    {
        SpawnNewBlock();

        if (triggerZone != null)
        {
            triggerZone.OnZoneClicked += DropBlock;
        }

        UpdateBlocksCounter();
    }

    private void OnDestroy()
    {
        if (triggerZone != null)
        {
            triggerZone.OnZoneClicked -= DropBlock;
        }
    }

    private void SpawnNewBlock()
    {
        currentBlock = Instantiate(blockPrefab, blockHolder);
        currentBlock.position = blockStartPosition;
        currentBlock.GetComponent<SpriteRenderer>().color = Random.ColorHSV();

        currentRigidbody = currentBlock.GetComponent<Rigidbody2D>();
        currentRigidbody.simulated = false;

        allBlocks.Add(currentBlock);

        blockSpeed += blockSpeedIncrement;
        isWaitingForNextBlock = false;

        UpdateBlocksCounter();
    }

    private void DropBlock()
    {
        if (currentBlock != null && currentRigidbody != null)
        {
            currentRigidbody.simulated = true;
            currentBlock = null;

            isWaitingForNextBlock = true;
            timer = timeBetweenBlocks;
        }
    }

    private void UpdateBlocksCounter()
    {
        if (blocksCounterText != null)
        {
            blocksCounterText.text = $"Блоків на платформі: {blocksInGame}";

            if (isWaitingForNextBlock)
            {
                blocksCounterText.text += $"\nНаступний блок: {timer:F1}с";
            }
        }
    }

    void Update()
    {
        if (currentBlock)
        {
            float moveAmount = Time.deltaTime * blockSpeed * blockDirection;
            currentBlock.position += new Vector3(moveAmount, 0, 0);

            if (Mathf.Abs(currentBlock.position.x) > xLimit)
            {
                currentBlock.position = new Vector3(blockDirection * xLimit, currentBlock.position.y, 0);
                blockDirection = -blockDirection;
            }
        }

        for (int i = allBlocks.Count - 1; i >= 0; i--)
        {
            Transform block = allBlocks[i];
            if (block == null) continue;

            if (block.position.y < fallThreshold)
            {
                if (countedBlocks.Contains(block))
                {
                    blocksInGame--;
                    countedBlocks.Remove(block);
                }

                allBlocks.RemoveAt(i);
                Destroy(block.gameObject);
                UpdateBlocksCounter();
            }
            else
            {
                if (!countedBlocks.Contains(block))
                {
                    Rigidbody2D rb = block.GetComponent<Rigidbody2D>();

                    if (rb != null && rb.linearVelocity.magnitude < 0.1f && rb.simulated)
                    {
                        blocksInGame++;
                        countedBlocks.Add(block);
                        UpdateBlocksCounter();
                    }
                }
            }
        }

        if (isWaitingForNextBlock)
        {
            timer -= Time.deltaTime;
            UpdateBlocksCounter();

            if (timer <= 0)
            {
                SpawnNewBlock();
            }
        }
    }
}