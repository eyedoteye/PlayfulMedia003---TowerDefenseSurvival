﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
  public int columns;
  public int rows;

  public GameObject tile_base;
  public LayerMask tile_layerMask;

  private TileController[,] tileControllers;

  void
  Awake()
  {
    tileControllers = new TileController[columns, rows];

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

        TileController groundTile_properties = CreateTile(
          columnIndex - columns / 2,
          rowIndex - rows / 2);

        tileControllers[columnIndex, rowIndex] = groundTile_properties;

        // Skip middle 4 tiles,
        // perhaps later develop a method that makes level design easier?
        if (rowIndex == rows / 2 || rowIndex == rows / 2 - 1)
          if (columnIndex == columns / 2 || columnIndex == columns / 2 - 1)
            groundTile_properties.blocked = true;
      }
    }
	}

  private TileController
  CreateTile(int column, int row)
  {
    Vector3 tilePosition = new Vector3(column + 0.5f, 0f, row + 0.5f);

    GameObject tile = Instantiate<GameObject>(tile_base);
    tile.transform.SetParent(transform, false);
    tile.transform.position = tilePosition;
    tile.SetActive(true);

    TileController tileProperties =
      tile.GetComponent<TileController>();
    tileProperties.blocked = false;
    tileProperties.col = column;
    tileProperties.row = row;

    return tileProperties;
  }

  public TileController
  get_groundTile_from_mouseCoords(
    Camera camera,
    Vector2 mouseCoords)
  {
    RaycastHit tileHit;
    bool tile_is_hit = Physics.Raycast(
      camera.ScreenPointToRay(mouseCoords),
      out tileHit,
      Mathf.Infinity,
      tile_layerMask);

    if(!tile_is_hit)
      return null;

    return tileHit.transform.gameObject.GetComponent<TileController>();
  }

  public void
  create_tower_on_tile(
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
  remove_tower_on_tile(TileController tileController)
  {
    Destroy(tileController.attachedBuilding);
    tileController.attachedBuilding = null;
  }

  public void
  update_all_towers(TowerController towerController_base)
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
        GameObject tower = tileControllers[columnIndex, rowIndex].attachedBuilding;
        if(tower != null)
        {
          tower.GetComponent<TowerController>().matchUpgrade(towerController_base);
        }
      }
    }
  }
}