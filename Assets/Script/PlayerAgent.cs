using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PlayerAgent : Agent
{
    [SerializeField]
    private Team team;
    
    
    //Before step 0 of the first game
    public override void Initialize()
    {
        
    }

    //What happens at the start of each game
    public override void OnEpisodeBegin()
    {
        
    }


    //What can the agent 'see'
    public override void CollectObservations(VectorSensor sensor)
    {
        
    }

    //What Can the Agent Do when an action (decision) is requested from it
    //we want a discrete descision
    public override void OnActionReceived(float[] vectorAction)
    {
        //How many possible moves are there in chess (we are looking for the smallest answer)
        //If we ignore a piece's position on the board, Considering only normal chess setup pieces, pawns can promote so still need all 36 moves
        //8*36 pawn moves + 14*4 castle and bishop moves + 28 queen moves + 8 knight moves + 8 king moves + 2 castle moves = 390 possible outputs

        // Get the output of the ML index
        int decisionIndex = Mathf.FloorToInt(vectorAction[0]);
        (TileIndex pieceLocation, TileIndex pieceDestination) = GetMoveFromAction(decisionIndex); //pass these to the movement manager
    }

    //Set a mask describing what output actions are valid for the next decision
    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        //Mask out any actions not in the AllMoves returned from GetValidMoves() 
    }

    //Probably dont want to override this, can be used to apply logic overrides to outputs and ignore Action mask
    //e.g. a player is up against a wall, set movement output in that direction to 0
    //public override void Heuristic(float[] actionsOut)
    //{
    //    
    //}

    //Converts from flat float array to index of the piece moving and the location to move to 
    //(if masking is working properly, it should already be a valid move)
    private (TileIndex piece, TileIndex move) GetMoveFromAction(int actionIndex)
    {
        //TODO
        //Define How Action array is split up
        int[] startIndicies = new int[16];
        //Pawns 0-7
        int index = 0;
        for(int i = 0; i<8; i++)
        {
            startIndicies[i] = index;
            index += 36;
        }
        //Knights 8-9
        
        //Bishops 10-11
        //Rooks 12-13
        //Queen 14
        //King 15

        return (TileIndex.Null, TileIndex.Null);
    }

    //Defines how each move 
    private TileIndex GetMoveFromIndex(int index, PieceType type)
    {
        //TODO
        return TileIndex.Null;
    }

    //Converts 
    private float[] FlattenMoveArray(float[][] inputArray)
    {
        //Size is fixed to 390
        //TODO
        return null;
    }
}
