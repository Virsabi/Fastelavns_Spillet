Only place scripts and content in here that is never referenced during build. Extensions that are referenced by scripts in runtime should stay in the runtime folder.

In other words, if the whole script can be encapsulated by 
#if UNITY_EDITOR 
*script* 
#endif 
and not make any errors during build, then the script can be palced in here.