/* Module      : IGoldHandler
 * Author      : Tim Calvert, Josh Morse
 * Email       : tncalvert@wpi.edu, jbmorse@wpi.edu
 * Course      : IMGD 4000
 *
 * Description : Interface for gold handling so we can access points through either script
 *
 * Date        : 2014/05/06
 *
 * (c) Copyright 2014, Worcester Polytechnic Institute.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Interface for gold handling
/// </summary>
public interface IGoldHandler {

	int GetPoints();
}
