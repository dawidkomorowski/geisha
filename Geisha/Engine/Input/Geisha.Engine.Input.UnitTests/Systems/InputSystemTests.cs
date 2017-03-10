using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input.Systems;
using Geisha.Framework.Input;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Systems
{
    [TestFixture]
    public class InputSystemTests
    {
        private const double DeltaTime = 0.1;
        private IInputProvider _inputProvider;
        private InputSystem _inputSystem;

        [SetUp]
        public void SetUp()
        {
            _inputProvider = Substitute.For<IInputProvider>();
            _inputSystem = new InputSystem(_inputProvider);
        }

        [Test]
        public void Update_ShouldCaptureHardwareInputOnce()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithInputComponents();

            // Act
            _inputSystem.Update(scene, DeltaTime);

            // Assert
            _inputProvider.Received(1).Capture();
        }

        [Test]
        public void Update_ShouldSetHardwareInputOnAllInputComponents()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithInputComponents();

            var hardwareInput = HardwareInput.Empty;
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.Update(scene, DeltaTime);

            // Assert
            Assert.That(scene.InputComponentOfEntity1.HardwareInput, Is.EqualTo(hardwareInput));
            Assert.That(scene.InputComponentOfEntity2.HardwareInput, Is.EqualTo(hardwareInput));
            Assert.That(scene.InputComponentOfEntity3.HardwareInput, Is.EqualTo(hardwareInput));
        }

        [TestCase(false, false, false, false, false, false, false)]
        [TestCase(true, true, true, true, true, true, true)]
        [TestCase(true, false, true, false, true, false, true)]
        [TestCase(false, true, false, true, false, true, true)]
        public void Update_ShouldSetActionStatesAccordingToHardwareInputAndInputMapping(bool right, bool left, bool up,
            bool space, bool expectedRight, bool expectedLeft, bool expectedJump)
        {
            // Arrange
            var scene = new SceneWithSampleActionMappings();

            var hardwareInput = new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Right] = right,
                [Key.Left] = left,
                [Key.Up] = up,
                [Key.Space] = space
            }));
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.Update(scene, DeltaTime);

            // Assert
            Assert.That(scene.InputComponent.GetActionState(scene.MoveRight.ActionName), Is.EqualTo(expectedRight));
            Assert.That(scene.InputComponent.GetActionState(scene.MoveLeft.ActionName), Is.EqualTo(expectedLeft));
            Assert.That(scene.InputComponent.GetActionState(scene.Jump.ActionName), Is.EqualTo(expectedJump));
        }

        [TestCase(false, false, false, false, false, 0, 0)]
        [TestCase(true, true, true, true, false, 0, 0)]
        [TestCase(true, false, true, false, false, 1, 1)]
        [TestCase(false, true, false, true, false, -1, -1)]
        [TestCase(false, false, false, false, true, 5, 0)]
        [TestCase(true, false, false, false, true, 6, 0)]
        public void Update_ShouldSetAxisStatesAccordingToHardwareInputAndInputMapping(bool up, bool down, bool right,
            bool left, bool space, double expectedUp, double expectedRight)
        {
            // Arrange
            var scene = new SceneWithSampleAxisMappings();

            var hardwareInput = new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = up,
                [Key.Down] = down,
                [Key.Right] = right,
                [Key.Left] = left,
                [Key.Space] = space
            }));
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.Update(scene, DeltaTime);

            // Assert
            Assert.That(scene.InputComponent.GetAxisState(scene.MoveUp.AxisName), Is.EqualTo(expectedUp));
            Assert.That(scene.InputComponent.GetAxisState(scene.MoveRight.AxisName), Is.EqualTo(expectedRight));
        }

        [TestCase(false, false, false, false, 0, 0, 0)]
        [TestCase(true, true, true, true, 1, 1, 1)]
        [TestCase(true, false, true, false, 1, 0, 1)]
        [TestCase(false, true, false, true, 0, 1, 1)]
        public void Update_ShouldCallActionBindingsAccordingToHardwareInputAndInputMapping(bool right, bool left,
            bool up, bool space, int expectedRightCount, int expectedLeftCount, int expectedJumpCount)
        {
            // Arrange
            var scene = new SceneWithSampleActionMappings();

            var moveRightCallCounter = 0;
            var moveLeftCallCounter = 0;
            var jumpCallCounter = 0;

            scene.InputComponent.BindAction(scene.MoveRight.ActionName, () => { moveRightCallCounter++; });
            scene.InputComponent.BindAction(scene.MoveLeft.ActionName, () => { moveLeftCallCounter++; });
            scene.InputComponent.BindAction(scene.Jump.ActionName, () => { jumpCallCounter++; });

            var hardwareInput = new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Right] = right,
                [Key.Left] = left,
                [Key.Up] = up,
                [Key.Space] = space
            }));
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.Update(scene, DeltaTime);

            // Assert
            Assert.That(moveRightCallCounter, Is.EqualTo(expectedRightCount));
            Assert.That(moveLeftCallCounter, Is.EqualTo(expectedLeftCount));
            Assert.That(jumpCallCounter, Is.EqualTo(expectedJumpCount));
        }

        [TestCase(false, false, false, false, false, 0, 0)]
        [TestCase(true, true, true, true, false, 0, 0)]
        [TestCase(true, false, true, false, false, 1, 1)]
        [TestCase(false, true, false, true, false, -1, -1)]
        [TestCase(false, false, false, false, true, 5, 0)]
        [TestCase(true, false, false, false, true, 6, 0)]
        public void Update_ShouldCallAxisBindingsAccordingToHardwareInputAndInputMapping(bool up, bool down, bool right,
            bool left, bool space, double expectedUp, double expectedRight)
        {
            // Arrange
            var scene = new SceneWithSampleAxisMappings();

            var moveUpCallCounter = 0;
            var moveRightCallCounter = 0;

            var moveUpState = 0.0;
            var moveRight = 0.0;

            scene.InputComponent.BindAxis(scene.MoveUp.AxisName, value =>
            {
                moveUpCallCounter++;
                moveUpState = value;
            });
            scene.InputComponent.BindAxis(scene.MoveRight.AxisName, value =>
            {
                moveRightCallCounter++;
                moveRight = value;
            });

            var hardwareInput = new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = up,
                [Key.Down] = down,
                [Key.Right] = right,
                [Key.Left] = left,
                [Key.Space] = space
            }));
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.Update(scene, DeltaTime);

            // Assert
            Assert.That(moveUpCallCounter, Is.EqualTo(1));
            Assert.That(moveRightCallCounter, Is.EqualTo(1));

            Assert.That(moveUpState, Is.EqualTo(expectedUp));
            Assert.That(moveRight, Is.EqualTo(expectedRight));
        }

        [TestCase(false, false, 0)]
        [TestCase(false, true, 1)]
        [TestCase(true, true, 0)]
        [TestCase(true, false, 0)]
        public void Update_ShouldCallActionBindingsCorrectNumberOfTimes_WhenExecutedTwice(bool first, bool second,
            int expectedCount)
        {
            // Arrange
            var scene = new SceneWithSampleActionMappings();

            var hardwareInput1 = new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = false,
                [Key.Down] = false,
                [Key.Right] = first,
                [Key.Left] = false,
                [Key.Space] = false
            }));

            var hardwareInput2 = new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = false,
                [Key.Down] = false,
                [Key.Right] = second,
                [Key.Left] = false,
                [Key.Space] = false
            }));

            // fill in action states based on hardwareInput
            _inputProvider.Capture().Returns(hardwareInput1);
            _inputSystem.Update(scene, DeltaTime);

            var callCounter = 0;
            scene.InputComponent.BindAction(scene.MoveRight.ActionName, () => { callCounter++; });

            // Act
            _inputProvider.Capture().Returns(hardwareInput1);
            _inputSystem.Update(scene, DeltaTime);

            _inputProvider.Capture().Returns(hardwareInput2);
            _inputSystem.Update(scene, DeltaTime);

            // Assert
            Assert.That(callCounter, Is.EqualTo(expectedCount));
        }

        [Test]
        public void Update_ShouldCallAxisBindingsEachTimeRegardlessHardwareInput()
        {
            // Arrange
            var scene = new SceneWithSampleAxisMappings();

            var callCounter = 0;
            scene.InputComponent.BindAxis(scene.MoveUp.AxisName, value => { callCounter++; });

            var allFalseHardwareInput = new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = false,
                [Key.Down] = false,
                [Key.Right] = false,
                [Key.Left] = false,
                [Key.Space] = false
            }));

            var allTrueHardwareInput = new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = true,
                [Key.Down] = true,
                [Key.Right] = true,
                [Key.Left] = true,
                [Key.Space] = true
            }));


            // Act
            for (var i = 0; i < 10; i++)
            {
                _inputProvider.Capture().Returns(allFalseHardwareInput);
                _inputSystem.Update(scene, DeltaTime);

                _inputProvider.Capture().Returns(allTrueHardwareInput);
                _inputSystem.Update(scene, DeltaTime);

                _inputProvider.Capture().Returns(allTrueHardwareInput);
                _inputSystem.Update(scene, DeltaTime);

                _inputProvider.Capture().Returns(allFalseHardwareInput);
                _inputSystem.Update(scene, DeltaTime);
            }

            // Assert
            Assert.That(callCounter, Is.EqualTo(40));
        }

        // TODO Remove FixedUpdate tests if no longer necessary or add missing tests that exist for Update
        [Test]
        public void FixedUpdate_ShouldCaptureHardwareInputOnce()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithInputComponents();

            // Act
            _inputSystem.FixedUpdate(scene);

            // Assert
            _inputProvider.Received(1).Capture();
        }

        [Test]
        public void FixedUpdate_ShouldSetHardwareInputOnAllInputComponents()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithInputComponents();

            var hardwareInput = HardwareInput.Empty;
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.FixedUpdate(scene);

            // Assert
            Assert.That(scene.InputComponentOfEntity1.HardwareInput, Is.EqualTo(hardwareInput));
            Assert.That(scene.InputComponentOfEntity2.HardwareInput, Is.EqualTo(hardwareInput));
            Assert.That(scene.InputComponentOfEntity3.HardwareInput, Is.EqualTo(hardwareInput));
        }

        private class SceneWithEntitiesWithInputComponents : Scene
        {
            public Entity EntityWithInputComponent1 { get; }
            public Entity EntityWithInputComponent2 { get; }
            public Entity EntityWithInputComponent3 { get; }

            public InputComponent InputComponentOfEntity1 { get; }
            public InputComponent InputComponentOfEntity2 { get; }
            public InputComponent InputComponentOfEntity3 { get; }

            public SceneWithEntitiesWithInputComponents()
            {
                RootEntity = new Entity();

                InputComponentOfEntity1 = new InputComponent();
                InputComponentOfEntity2 = new InputComponent();
                InputComponentOfEntity3 = new InputComponent();

                EntityWithInputComponent1 = new Entity {Parent = RootEntity};
                EntityWithInputComponent1.AddComponent(InputComponentOfEntity1);

                EntityWithInputComponent2 = new Entity {Parent = RootEntity};
                EntityWithInputComponent2.AddComponent(InputComponentOfEntity2);

                EntityWithInputComponent3 = new Entity {Parent = RootEntity};
                EntityWithInputComponent3.AddComponent(InputComponentOfEntity3);
            }
        }

        private class SceneWithSampleActionMappings : Scene
        {
            public InputComponent InputComponent { get; }

            public ActionMappingGroup MoveRight { get; }
            public ActionMappingGroup MoveLeft { get; }
            public ActionMappingGroup Jump { get; }

            public SceneWithSampleActionMappings()
            {
                RootEntity = new Entity();

                MoveRight = new ActionMappingGroup {ActionName = nameof(MoveRight)};
                MoveRight.ActionMappings.Add(new ActionMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Right}
                });

                MoveLeft = new ActionMappingGroup {ActionName = nameof(MoveLeft)};
                MoveLeft.ActionMappings.Add(new ActionMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Left}
                });

                Jump = new ActionMappingGroup {ActionName = nameof(Jump)};
                Jump.ActionMappings.Add(new ActionMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Up}
                });
                Jump.ActionMappings.Add(new ActionMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Space}
                });

                var inputMapping = new InputMapping();
                inputMapping.ActionMappingGroups.Add(MoveRight);
                inputMapping.ActionMappingGroups.Add(MoveLeft);
                inputMapping.ActionMappingGroups.Add(Jump);

                InputComponent = new InputComponent {InputMapping = inputMapping};

                var entity = new Entity {Parent = RootEntity};
                entity.AddComponent(InputComponent);
            }
        }

        private class SceneWithSampleAxisMappings : Scene
        {
            public InputComponent InputComponent { get; }

            public AxisMappingGroup MoveUp { get; }
            public AxisMappingGroup MoveRight { get; }

            public SceneWithSampleAxisMappings()
            {
                RootEntity = new Entity();

                MoveUp = new AxisMappingGroup {AxisName = nameof(MoveUp)};
                MoveUp.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Up},
                    Scale = 1.0
                });
                MoveUp.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Down},
                    Scale = -1.0
                });
                MoveUp.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Space},
                    Scale = 5.0
                });

                MoveRight = new AxisMappingGroup {AxisName = nameof(MoveRight)};
                MoveRight.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Right},
                    Scale = 1.0
                });
                MoveRight.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Left},
                    Scale = -1.0
                });

                var inputMapping = new InputMapping();
                inputMapping.AxisMappingGroups.Add(MoveUp);
                inputMapping.AxisMappingGroups.Add(MoveRight);

                InputComponent = new InputComponent {InputMapping = inputMapping};

                var entity = new Entity {Parent = RootEntity};
                entity.AddComponent(InputComponent);
            }
        }
    }
}