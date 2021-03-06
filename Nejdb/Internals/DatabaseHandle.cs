﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Nejdb.Internals
{
    internal class DatabaseHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        //Prevents out of order garbage collector
        internal readonly LibraryHandle LibraryHandle;

        private NewInstanceDelegate _newInstance;
        private DeleteInstanceDelegate _deleteInstance;

        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbnew", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern IntPtr _ejdbnew();
        //Creates new instance of ejdb. Don't know what' is it, but it looks it should be done before opening database
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbnew")]
        private delegate IntPtr NewInstanceDelegate(LibraryHandle handle);

        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbdel", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern IntPtr _ejdbdel([In] IntPtr db);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbdel")]
        private delegate void DeleteInstanceDelegate([In]IntPtr database);

        public DatabaseHandle(LibraryHandle libraryHandle) : base(true)
        {
            LibraryHandle = libraryHandle;
            _newInstance = libraryHandle.GetUnmanagedDelegate<NewInstanceDelegate>();
            _deleteInstance = libraryHandle.GetUnmanagedDelegate<DeleteInstanceDelegate>();

            this.handle = _newInstance(LibraryHandle);

            if (IsInvalid)
            {
                throw new EjdbException("Unable to create ejdb instance");
            }
        }

        protected override bool ReleaseHandle()
        {
            _deleteInstance(this.handle);

            //makes GC live easier
            _newInstance = null;
            _deleteInstance = null;
            return true;
        }
    }
}