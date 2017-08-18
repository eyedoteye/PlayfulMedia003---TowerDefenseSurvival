using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
WorldController : MonoBehaviour
{
  public int columns;
  public int rows;
  public int tileDivisions; 

  public GameObject baseTile;
  public LayerMask tileLayerMask;

  private TileController[,] tileControllers;
  private int trueColumns;
  private int trueRows;

  void
  Awake()
  {
    trueColumns = columns * tileDivisions;
    trueRows = rows * tileDivisions;

    tileControllers = new TileController[trueColumns, trueRows];

    for(
      int columnIndex = 0;
      columnIndex < trueColumns;
      ++columnIndex)
    {
      for(
        int rowIndex = 0;
        rowIndex < trueRows;
        ++rowIndex)
      {
        TileController groundTileController = CreateTile(
          columnIndex - trueColumns / 2,
          rowIndex - trueRows / 2);

        groundTileController.DestroyIfCollidingWithBuilding();

        tileControllers[columnIndex, rowIndex] = groundTileController;
      }
    }
	}

  private TileController
  CreateTile(int column, int row)
  {
    float tileScale = 1f / tileDivisions;

    Vector3 tilePosition = new Vector3(
      (column + 0.5f) * tileScale,
      0f,
      (row + 0.5f) * tileScale);

    GameObject tile = Instantiate<GameObject>(baseTile);
    tile.transform.SetParent(transform, false);
    tile.transform.position = tilePosition;
    tile.transform.localScale = new Vector3(tileScale, tileScale, 1f); 
    tile.SetActive(true);

    TileController tileProperties =
      tile.GetComponent<TileController>();
    tileProperties.blocked = false;
    tileProperties.col = column;
    tileProperties.row = row;

    return tileProperties;
  }

  public TileController
  GetGroundTile(
    Camera camera,
    Vector2 mouseCoords)
  {
    RaycastHit tileHit;
    bool tile_is_hit = Physics.Raycast(
      camera.ScreenPointToRay(mouseCoords),
      out tileHit,
      Mathf.Infinity,
      tileLayerMask);

    if(!tile_is_hit)
      return null;

    return tileHit.transform.gameObject.GetComponent<TileController>();
  }

  public void
  CreateTower(
    GameObject tower_base,
    TileController tileController)
  {
    GameObject tower = Instantiate(tower_base);
    tower.transform.position =
      tileController.transform.position + tower_base.transform.position;
    tower.SetActive(true);

    tileController.attachedBuilding = tower;
  }

  public void
  RemoveTower(TileController tileController)
  {
    Destroy(tileController.attachedBuilding);
    tileController.attachedBuilding = null;
  }

  public void
  UpdateAllTowers(TowerController baseTowerController)
  {
    for(
      int columnIndex = 0;
      columnIndex < columns;
      ++columnIndex)
    {
      for(
        int rowIndex = 0;
        rowIndex < rows;
        ++rowIndex)
      {
        TileController tileController = tileControllers[columnIndex, rowIndex];
        if(tileController == null)
          continue;

        GameObject tower = tileController.attachedBuilding;
        if(tower != null)
          tower.GetComponent<TowerController>().MatchUpgrade(baseTowerController);
      }
    }
  }
}