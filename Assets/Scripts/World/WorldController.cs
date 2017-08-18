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
          columnIndex,
          rowIndex);

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
      (column - trueColumns / 2f) * tileScale,
      0f,
      (row - trueRows / 2f) * tileScale);

    GameObject tile = Instantiate<GameObject>(baseTile);
    tile.transform.SetParent(transform, false);
    tile.transform.position = tilePosition;
    tile.transform.localScale = new Vector3(tileScale, tileScale, 1f); 
    tile.SetActive(true);

    TileController tileProperties =
      tile.GetComponent<TileController>();
    tileProperties.blocked = false;
    tileProperties.column = column;
    tileProperties.row = row;

    return tileProperties;
  }

  public TileController
  GetTileFromMousePosition(
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

  public bool
  TilesAreValidForBuilding(TileController[] tileControllersForBuilding)
  {
    if(tileControllersForBuilding == null)
      return false;

    for(
      int tileIndex = 0;
      tileIndex < tileControllersForBuilding.Length;
      ++tileIndex)
    {
      TileController checkeeTileController = tileControllersForBuilding[tileIndex];
      if(checkeeTileController == null)
        return false;
      
      if(checkeeTileController.blocked)
        return false;
      
      if(checkeeTileController.attachedBuilding != null)
        return false;
    }

    return true;
  }

  public TileController[]
  GetTilesForBuilding(
    TileController centerTileController,
    TowerController towerController)
  {
    if(centerTileController == null)
      return null;

    TileController[] tileControllersForBuilding = new TileController[
      towerController.subtileWidth * towerController.subtileHeight];
    int tileIndex = 0;

    int halfWidth = towerController.subtileWidth / 2;

    int startColumnIndex = centerTileController.column - halfWidth;
    if(startColumnIndex < 0)
      startColumnIndex = 0;
    int endColumnIndex = centerTileController.column + halfWidth;
    if(endColumnIndex >= trueColumns)
      endColumnIndex = trueColumns - 1;

    int halfHeight = towerController.subtileHeight / 2;

    int startRowIndex = centerTileController.row - halfHeight;
    if(startRowIndex < 0)
      startRowIndex = 0;
    int endRowIndex = centerTileController.row + halfHeight;
    if(endRowIndex >= trueRows)
      endRowIndex = trueRows - 1;

    for(
      int columnIndex = startColumnIndex;
      columnIndex <= endColumnIndex;
      ++columnIndex)
    {
      for(
        int rowIndex = startRowIndex;
        rowIndex <= endRowIndex;
        ++rowIndex)
      {
        tileControllersForBuilding[tileIndex] = tileControllers[columnIndex, rowIndex];
        ++tileIndex;
      }
    }

    return tileControllersForBuilding;
  }

  public void
  CreateTower(
    GameObject baseTower,
    TileController centerTileControllerForBuilding,
    TileController[] tileControllersForBuilding)
  {
    GameObject tower = Instantiate(baseTower);
    tower.transform.position =
      centerTileControllerForBuilding.transform.position + baseTower.transform.position;
    tower.SetActive(true);

    for(
      int tileIndex = 0;
      tileIndex < tileControllersForBuilding.Length;
      ++tileIndex)
    {
      TileController tileController = tileControllersForBuilding[tileIndex];
      if(tileController != null)
        tileControllersForBuilding[tileIndex].attachedBuilding = tower;
    }
  }

  public void
  RemoveTower(
    TileController  centerTileControllerForBuilding,
    TileController[] tileControllersForBuilding)
  {
    Destroy(centerTileControllerForBuilding.attachedBuilding);

    for(
      int tileIndex = 0;
      tileIndex < tileControllersForBuilding.Length;
      ++tileIndex)
    {
      TileController tileController = tileControllersForBuilding[tileIndex];
      if(tileController != null)
        tileControllersForBuilding[tileIndex].attachedBuilding = null;
    }

  }

  public void
  UnhilightAllTowers()
  {
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
        TileController tileController = tileControllers[columnIndex, rowIndex];
        if(tileController == null)
          continue;

        tileController.GetComponent<MeshRenderer>().material.color = Color.white;
        if(tileController.attachedBuilding != null)
          tileController.attachedBuilding.GetComponent<MeshRenderer>().material.color =
            tileController.attachedBuilding.GetComponent<TowerController>().color;
      }
    }
  }

  public void
  UpdateAllTowers(TowerController baseTowerController)
  {
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