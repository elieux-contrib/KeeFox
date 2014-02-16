using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.ComponentModel.Design;

namespace KeePassRPC
{
    [Designer(typeof(LocalizerDesigner))]
    [DesignerSerializer(typeof(LocalizerSerializer), typeof(CodeDomSerializer))]
    public class Localizer : Component
    {
    }

    public class LocalizerSerializer : CodeDomSerializer
    {
        public override object Deserialize(IDesignerSerializationManager manager, object codeDomObject)
        {
            CodeDomSerializer baseSerializer = (CodeDomSerializer)
                manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
            return baseSerializer.Deserialize(manager, codeDomObject);
        }

        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            CodeDomSerializer baseSerializer =
                (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));

            object codeObject = baseSerializer.Serialize(manager, value);

            if (codeObject is CodeStatementCollection)
            {
                CodeStatementCollection statements = (CodeStatementCollection)codeObject;
                CodeTypeDeclaration classTypeDeclaration =
                    (CodeTypeDeclaration)manager.GetService(typeof(CodeTypeDeclaration));
                CodeExpression typeofExpression = new CodeTypeOfExpression(classTypeDeclaration.Name);
                CodeExpression rightCodeExpression = new CodeObjectCreateExpression("SingleAssemblyComponentResourceManager", new CodeExpression[] { typeofExpression });
                CodeAssignStatement assign = new CodeAssignStatement(new CodeVariableReferenceExpression("resources"), rightCodeExpression);

                statements.Insert(0, assign);
            }
            return codeObject;
        }
    }

    public class LocalizerDesigner : ComponentDesigner
    {
        public override void Initialize(IComponent c)
        {
            base.Initialize(c);
            var dh = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (dh == null)
                return;

            var innerListProperty = dh.Container.Components.GetType().GetProperty("InnerList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy);
            var innerList = innerListProperty.GetValue(dh.Container.Components, null) as System.Collections.ArrayList;
            if (innerList == null)
                return;
            if (innerList.IndexOf(c) <= 1)
                return;
            innerList.Remove(c);
            innerList.Insert(0, c);

        }
    }


}