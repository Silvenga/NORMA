//------------------------------------------------------------------------------
/// <copyright from='1997' to='2003' company='Microsoft Corporation'>           
/// Copyright (c) Microsoft Corporation. All Rights Reserved.                
/// Information Contained Herein is Proprietary and Confidential.            
/// </copyright>                                                                
//------------------------------------------------------------------------------

///////////////////////////////////////////////////////////////////////////////
// Menu IDs          (0x01xx)
// IMPORTANT: keep constants in sync with ORMModel\Shell\ORMCommandSet.cs
#define menuIdContextMenu				0x0100

///////////////////////////////////////////////////////////////////////////////
// Menu Group IDs    (0x10xx)
#ifdef DEBUG
#define groupIdDebugCommands			0x0FFF
#endif
#define groupIdDeleteContext			0x1000
#define groupIdFactCommands			0x1001
#define groupIdRoleCommands			0x1002
#define groupIdModelCommands			0x1003

///////////////////////////////////////////////////////////////////////////////
// Command IDs       (0x2xxx)
// IMPORTANT: keep constants in sync with ORMModel\Shell\ORMCommandSet.cs
#ifdef DEBUG
#define cmdIdDebugViewStore			0x28FF
#endif
#define cmdIdViewModelExplorer			0x2900
#define cmdIdViewReadingEditor			0x2901
#define cmdIdViewReferenceModeEditor		0x2902
#define cmdIdInsertRoleAfter			0x2903
#define cmdIdInsertRoleBefore			0x2904
#define cmdIdViewFactEditor			0x2905
///////////////////////////////////////////////////////////////////////////////
// Bitmap IDs
