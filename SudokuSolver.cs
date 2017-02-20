using System;
using System.Linq;
using System.Collections.Generic;

// Based on the Article "Solving Every Sudoku Puzzle"
// by Peter Norvig
// nordvig.com/sudoku.html

// Rewritten to C# by Einar Helle

static class SudokuSolver {

  static string rows = "ABCDEFGHI";
  static string cols = "123456789";
  static string digits = "123456789";
  static string[] squares = cross(rows, cols);

  // All constraints that have to be solved (rows, cols, squares)
  static IEnumerable<string[]> unitlist = 
      (from c in cols select cross(rows, c.ToString()))
      .Concat(from r in rows select cross(r.ToString(), cols))
      .Concat(from rs in new [] {"ABC", "DEF", "GHI"}
              from cs in new [] {"123", "456", "789"}
              select cross(rs, cs));
 
 // Dict of constraints for a particular square
 // 1 row, 1 col, 1 square
 static Dictionary<string, IEnumerable<string[]>> units =
  squares.ToDictionary(s => s, s => (
      from u in unitlist
      where u.Contains(s)
      select u));

  // Set of squares in units minus self 
  static Dictionary<string, HashSet<string>> peers = 
  squares.ToDictionary(s => s, s => 
      new HashSet<string>(
      from ul in units[s]
      from u in ul
      where u != s
      select u
    ));

  static string[] cross(string A, string B) {
    return (from a in A from b in B select ""+a+b).ToArray();
  }

  // Convert string representation of sudoku grid
  // to a Dictionary
  static Dictionary<string, string> parse_grid(string grid) {
    var values = squares.ToDictionary(s => s, _ => digits);
    foreach (var entry in grid_values(grid)) {
      if (digits.Contains(entry.Value) &&
          assign(values, entry.Key, entry.Value) == null) {
        return null;
      }
    }
    return values;
  }

  // Helper function to parse grid.
  static Dictionary<string, char> grid_values(string grid) {
    var chars = from c in grid 
                where (digits + "0.").Contains(c)
                select c;
    var res = squares.Zip(chars, (s,c) => new { s, c })
                     .ToDictionary(x => x.s, x => x.c);
    return res;
  }

  // Remove all other values except d from values[s]
  // Return null if contradiction, else values
  static Dictionary<string, string> assign(
    Dictionary<string, string> values, string s, char d) {
      var other_values = values[s].Replace(d.ToString(), "");
      foreach (var d2 in other_values) {
        if (eliminate(values, s, d2) == null) {
          return null;
        }
      }
      return values;
  }

  // Remove d from values[s], propagate when values or places <= 2
  // Return null if contradiction, else values
  static Dictionary<string, string> eliminate(
  Dictionary<string, string> values, string s, char d) {
    if (!values[s].Contains(d)) {
      return values; // Allready removed
    }
    values[s] = values[s].Replace(d.ToString(), "");
    if (values[s] == "") {
      return null; // Last value removed - Dead end.
    } 
    else if (values[s].Length == 1) {
      var d2 = values[s][0];
      foreach (var s2 in peers[s]) {
        if (eliminate(values, s2, d2) == null) {
          return null;
        }
      }
    }
    foreach (var u in units[s]) {
      var dplaces = (from s3 in u 
                    where values[s3].Contains(d)
                    select s3).ToArray();
      if (dplaces.Count() == 0) {
        return null;
      } 
      else if ((dplaces.Count() == 1) && assign(values, dplaces[0], d) == null) {
        return null;
      }
    }
    return values;
  }

  // The main method to solve a sudoku puzzle
  public static string solve(string grid) {
    return values2string(search(parse_grid(grid)));
  }

  // Recursive depth-first search 
  // order by shortest list of possible values
  // try to assign a variable and see if solves. 
  static Dictionary<string, string> search(Dictionary<string, string> values) {
    if (values == null) {
      return null;
    }
    if ((from s in squares where values[s].Length != 1 select s).Count() == 0) {
      return values; // Solved
    }
    var next = (from s in squares
                where values[s].Length > 1
                select new { len = values[s].Length, square = s })
                .Aggregate((x, y) => x.len > y.len ? x : y); // min value - rewrite?
    
    return some(from d in values[next.square]
                select search(assign(new Dictionary<string, string>(values),
                                      next.square, d)));
  }

  // Find element i squence that is not null
  static Dictionary<string, string> some(
    IEnumerable<Dictionary<string, string>> seq) {
    foreach (var elm in seq) {
      if (elm != null) {
        return elm;
      }
    }
    return null;
  }

  // Convert solution Dictionary to string representation
  public static string values2string(Dictionary<string, string> values) {
    if (values == null) {
      return "Not able to solve"; // throw error? Other return?
    } 
    else {
      return String.Join("", (from elm in values
                              orderby elm.Key[0], elm.Key[1]
                              select elm.Value));
    }
  }

  public static string prettyprint(string puzzle) {
    if (puzzle.Length == 9*9) {
      var res = "";
      for (var i = 0; i < 9*9; i++) {
        if (i != 0 && i % 27 == 0)
          res += "\n " + new String('-', 19) + "\n ";
        else if (i % 9 == 0)
          res += "\n ";
        else if (i % 3 == 0)
          res += "|";
        res += puzzle[i] + " ";
      }
      return res;
    }
    else {
      return puzzle;
    }
  }
}