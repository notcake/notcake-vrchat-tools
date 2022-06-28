# notcake.Unity.UnityPrefabFileIDDiff
Diffs the `fileID`s between two `.prefab` files.  
Both `.prefab` files must be well-formed.

Usage:
```
notcake.Unity.UnityPrefabFileIDDiff <left> <right>
```

Example output:
```
* Nested Prefab 2 &4989725120814738964 -> Nested Prefab 3 &6293954053335516682
  * Transform &8820928727568499859 -> &7497849768378543245
  * PrefabInstance &4156974465077255250 -> &1670981476826454699
    * Transform &4889415719643437249 -> &7879820769096490552 stripped
  * PrefabInstance &4431041678912474380 -> &1512319774972293766
    * Transform &5121722109705652639 -> &7968817328979179029 stripped
    > Transform &3804593009358545394 stripped
      > New Inner Child &6053085283548292871
        > Transform &643158201123562314
```
