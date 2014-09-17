using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Data;
using MyToolkit.Model;
using MyToolkit.Mvvm;
using MyToolkit.Tests.WinRT.Model;

namespace MyToolkit.Tests.WinRT.Data
{
    [TestClass]
    public class UndoRedoManagerTests
    {
        [TestMethod]
        public void When_changing_scalar_property_then_undo_should_work()
        {
            //// Arrange
            var obj = new MyObject();
            obj.Name = "A";
            var undoRedoManager = new UndoRedoManager(obj, new MockDispatcher());

            Assert.IsFalse(undoRedoManager.CanUndo);

            //// Act
            obj.Name = "B";

            Assert.AreEqual("B", obj.Name);
            Assert.IsTrue(undoRedoManager.CanUndo);

            undoRedoManager.Undo();
            Assert.IsFalse(undoRedoManager.CanUndo);

            //// Assert
            Assert.AreEqual("A", obj.Name);
        }

        [TestMethod]
        public void When_undo_a_scalar_property_then_redo_should_work()
        {
            //// Arrange
            var obj = new MyObject();
            obj.Name = "A";
            var undoRedoManager = new UndoRedoManager(obj, new MockDispatcher());

            Assert.IsFalse(undoRedoManager.CanRedo);
            Assert.IsFalse(undoRedoManager.CanUndo);

            //// Act
            obj.Name = "B";
            undoRedoManager.Undo();
            Assert.AreEqual("A", obj.Name);

            Assert.IsTrue(undoRedoManager.CanRedo);
            undoRedoManager.Redo();

            //// Assert
            Assert.AreEqual("B", obj.Name);
        }

        [TestMethod]
        public void When_changing_a_scalar_property_then_redo_should_not_work()
        {
            //// Arrange
            var obj = new MyObject();
            obj.Name = "A";
            var undoRedoManager = new UndoRedoManager(obj, new MockDispatcher());

            Assert.IsFalse(undoRedoManager.CanRedo);
            Assert.IsFalse(undoRedoManager.CanUndo);

            //// Act
            obj.Name = "B";
            undoRedoManager.Undo();

            Assert.IsTrue(undoRedoManager.CanRedo);
            obj.Name = "C";

            //// Assert
            Assert.IsFalse(undoRedoManager.CanRedo);
        }

        [TestMethod]
        public void When_adding_object_to_list_then_undo_should_work()
        {
            //// Arrange
            var obj = new MyObject();
            obj.List.Add(new MyObject());

            var undoRedoManager = new UndoRedoManager(obj, new MockDispatcher());
            Assert.IsFalse(undoRedoManager.CanUndo);
            Assert.AreEqual(1, obj.List.Count);

            //// Act
            obj.List.Add(new MyObject());

            Assert.AreEqual(2, obj.List.Count);
            Assert.IsTrue(undoRedoManager.CanUndo);

            undoRedoManager.Undo();

            //// Assert
            Assert.AreEqual(1, obj.List.Count);
        }

        [TestMethod]
        public void When_removing_object_from_list_then_undo_should_work()
        {
            //// Arrange
            var obj = new MyObject();
            obj.List.Add(new MyObject());
            obj.List.Add(new MyObject());

            var undoRedoManager = new UndoRedoManager(obj, new MockDispatcher());
            Assert.IsFalse(undoRedoManager.CanUndo);
            Assert.AreEqual(2, obj.List.Count);

            //// Act
            obj.List.RemoveAt(0);

            Assert.AreEqual(1, obj.List.Count);
            Assert.IsTrue(undoRedoManager.CanUndo);

            undoRedoManager.Undo();

            //// Assert
            Assert.AreEqual(2, obj.List.Count);
        }

        [TestMethod]
        public void When_clearing_a_list_then_undo_should_work()
        {
            //// Arrange
            var obj = new MyObject();
            obj.List.Add(new MyObject());
            obj.List.Add(new MyObject());

            var undoRedoManager = new UndoRedoManager(obj, new MockDispatcher());
            Assert.IsFalse(undoRedoManager.CanUndo);
            Assert.AreEqual(2, obj.List.Count);

            //// Act
            obj.List.Clear();

            Assert.AreEqual(0, obj.List.Count);
            Assert.IsTrue(undoRedoManager.CanUndo);

            undoRedoManager.Undo();

            //// Assert
            Assert.AreEqual(2, obj.List.Count);
        }

        [TestMethod]
        public void When_changing_property_of_excluded_child_type_then_undo_should_not_work()
        {
            //// Arrange
            var obj = new MyObject();
            obj.AddExcludedChildType(typeof(ObservableObject));
            obj.Child = new MyObject { Name = "A" };

            var undoRedoManager = new UndoRedoManager(obj, new MockDispatcher());
            Assert.IsFalse(undoRedoManager.CanUndo);

            //// Act
            obj.Child.Name = "B";

            Assert.AreEqual("B", obj.Child.Name);

            //// Assert
            Assert.IsFalse(undoRedoManager.CanUndo);
        }
    }

    public class MockDispatcher : IDispatcher
    {
        public void InvokeAsync(Action action)
        {
            action();
        }
    }
}