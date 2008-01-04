﻿using System;
using System.Collections.Generic;
using System.Text;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.PropertyGetSet
{
  [TestClass()]
  public class PropertyGetSetTests
  {
    [TestMethod]
    public void ExplicitFieldProperties()
    {
      EditableGetSet root = new EditableGetSet();
      root.PropertyChanging += new System.ComponentModel.PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
      Assert.AreEqual("n/a", root.F03, "Default value should have been set");
      Assert.AreEqual("", root.F01, "String should default to string.Empty");
      Assert.AreEqual(0, root.F02, "Numeric should default to 0");

      root.F01 = "hi there";
      Assert.AreEqual("hi there", root.F01, "String should have been set");
      Assert.AreEqual("F01", _changingName, "F01 should have been changing");
      Assert.AreEqual("F01", _changedName, "F01 should have changed");

      root.F02 = 123;
      Assert.AreEqual(123, root.F02, "Numeric should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.PropertyChanging -= new System.ComponentModel.PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
    }

    [TestMethod]
    public void ManagedFieldProperties()
    {
      EditableGetSet root = new EditableGetSet();
      root.PropertyChanging += new System.ComponentModel.PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
      Assert.AreEqual("n/a", root.M03, "Default value should have been set");
      Assert.AreEqual("", root.M01, "String should default to string.Empty");
      Assert.AreEqual(0, root.M02, "Numeric should default to 0");

      root.M01 = "hi there";
      Assert.AreEqual("hi there", root.M01, "String should have been set");
      Assert.AreEqual("M01", _changingName, "M01 should have been changing");
      Assert.AreEqual("M01", _changedName, "M01 should have changed");

      root.M02 = 123;
      Assert.AreEqual(123, root.M02, "Numeric should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.PropertyChanging -= new System.ComponentModel.PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
    }

    [TestMethod]
    public void MarkClean()
    {
      EditableGetSet root = new EditableGetSet();

      root.M01 = "hi there";
      root.F01 = "hi there";
      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.MarkClean();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
    }

    [TestMethod]
    public void SmartDateProperties()
    {
      EditableGetSet root = new EditableGetSet();
      Assert.AreEqual("", root.F04, "Field should default to string.Empty");
      Assert.AreEqual("", root.M04, "Should default to string.Empty");

      root.F04 = new DateTime(1998, 12, 21).ToShortDateString();
      Assert.AreEqual(new DateTime(1998, 12, 21).ToShortDateString(), root.F04, "Field SmartDate should have been set");

      root.M04 = new DateTime(1998,12,21).ToShortDateString();
      Assert.AreEqual(new DateTime(1998, 12, 21).ToShortDateString(), root.M04, "SmartDate should have been set");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
    }

    [TestMethod]
    public void SimpleChildProperties()
    {
      EditableGetSet root = new EditableGetSet();
      root.PropertyChanging += new System.ComponentModel.PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
      
      EditableGetSet child = root.C01;
      Assert.IsNotNull(child, "Child should not be null");
      Assert.AreEqual("C01", _changingName, "C01 should have been changing");
      Assert.AreEqual("C01", _changedName, "C01 should have changed");
      _changingName = "";
      _changedName = "";

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
      Assert.IsTrue(child.IsDirty, "Child should be dirty");

      child.F01 = "hi there";
      Assert.AreEqual("", _changingName, "C01 should NOT have been changing");
      Assert.AreEqual("C01", _changedName, "C01 should have changed");

      root.PropertyChanging -= new System.ComponentModel.PropertyChangingEventHandler(root_PropertyChanging);
      root.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(root_PropertyChanged);
    }

    [TestMethod]
    public void RootUndoCancel()
    {
      EditableGetSet root = new EditableGetSet();

      Assert.IsFalse(root.IsDirty, "Root should not start dirty");

      Assert.AreEqual("", root.F01, "Explicit String should default to string.Empty");
      Assert.AreEqual("", root.M01, "Managed String should default to string.Empty");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after defaults load");

      root.BeginEdit();
      root.F01 = "f01";
      root.M01 = "m01";
      Assert.AreEqual("f01", root.F01, "String should be f01");
      Assert.AreEqual("m01", root.M01, "String should be m01");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.CancelEdit();
      Assert.AreEqual("", root.F01, "Explicit String should revert to string.Empty");
      Assert.AreEqual("", root.M01, "Managed String should revert to string.Empty");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
    }

    [TestMethod]
    public void RootUndoApply()
    {
      EditableGetSet root = new EditableGetSet();

      Assert.IsFalse(root.IsDirty, "Root should not start dirty");

      Assert.AreEqual("", root.F01, "Explicit String should default to string.Empty");
      Assert.AreEqual("", root.M01, "Managed String should default to string.Empty");

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after defaults load");

      root.BeginEdit();
      root.F01 = "f01";
      root.M01 = "m01";
      Assert.AreEqual("f01", root.F01, "String should be f01");
      Assert.AreEqual("m01", root.M01, "String should be m01");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.ApplyEdit();
      Assert.AreEqual("f01", root.F01, "String should be f01 after apply");
      Assert.AreEqual("m01", root.M01, "String should be m01 after apply");

      Assert.IsTrue(root.IsDirty, "Root should be dirty after ApplyEdit");
      Assert.IsTrue(root.IsValid, "Root should be valid (no validation rules exist)");

      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after Save");
    }

    [TestMethod]
    public void RootChildUndoCancel()
    {
      EditableGetSet root = new EditableGetSet();
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");

      EditableGetSet initialChild = root.C01;
      Assert.AreEqual(1, initialChild.EditLevel, "Child edit level after being created");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.CancelEdit();
      Assert.AreEqual(0, root.EditLevel, "Root edit level after CancelEdit");
      EditableGetSet secondChild = root.C01;
      Assert.AreEqual(0, secondChild.EditLevel, "Second child edit level after being created");
      Assert.IsFalse(ReferenceEquals(initialChild, secondChild), "Child objects should be different");

      Assert.IsTrue(root.IsDirty, "Root should be dirty after second child created");
    }

    [TestMethod]
    public void RootChildUndoCancelIsDirty()
    {
      EditableGetSet root = new EditableGetSet();
      root.BeginEdit();

      EditableGetSet initialChild = root.C01;

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
      Assert.IsTrue(initialChild.IsDirty, "Child should be dirty");

      root.CancelEdit();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
    }

    [TestMethod]
    public void RootChildUndoApply()
    {
      EditableGetSet root = new EditableGetSet();
      Assert.AreEqual(0, root.EditLevel, "Root edit level before BeginEdit");
      root.BeginEdit();
      Assert.AreEqual(1, root.EditLevel, "Root edit level after BeginEdit");

      EditableGetSet initialChild = root.C01;
      Assert.AreEqual(1, initialChild.EditLevel, "Child edit level after being created");

      Assert.IsTrue(root.IsDirty, "Root should be dirty");

      root.ApplyEdit();
      Assert.AreEqual(0, root.EditLevel, "Root edit level after ApplyEdit");
      EditableGetSet secondChild = root.C01;
      Assert.AreEqual(0, secondChild.EditLevel, "Second child edit level after ApplyEdit");
      Assert.IsTrue(ReferenceEquals(initialChild, secondChild), "Child objects should be the same");

      Assert.IsTrue(root.IsDirty, "Root should be dirty after ApplyEdit");

      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after Save");
    }

    // ======================================================================================
    // ======================================================================================

    private string _changingName;
    private string _changedName;
    void root_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
    {
      _changingName = e.PropertyName;
    }

    void root_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      _changedName = e.PropertyName;
    }

  }
}