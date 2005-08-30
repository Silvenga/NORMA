<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:plx="http://Schemas.Neumont.edu/CodeGeneration/Plix"
    xmlns:se="http://Schemas.Neumont.edu/Private/SerializationExtensions"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt">
	<!-- Pick up param value supplied automatically by plix loader -->
	<xsl:param name="CustomToolNamespace" select="'TestNamespace'"/>
	<xsl:template match="se:CustomSerializedElements">
		<plx:Root xmlns:plx="http://Schemas.Neumont.edu/CodeGeneration/Plix">
			<plx:Using name="System"/>
			<plx:Using name="System.Collections"/>
			<plx:Using name="System.Collections.ObjectModel"/>
			<plx:Using name="System.Collections.Generic"/>
			<plx:Using name="Microsoft.VisualStudio.Modeling"/>
			<plx:Using name="Microsoft.VisualStudio.Modeling.Diagrams"/>
			<plx:Using name="Neumont.Tools.ORM.Shell"/>
			<plx:Namespace name="{$CustomToolNamespace}">
				<xsl:apply-templates select="child::*"/>
			</plx:Namespace>
		</plx:Root>
	</xsl:template>
	<xsl:template match="se:Element">
		<xsl:variable name="ClassName" select="@Class"/>
		<xsl:variable name="ClassOverride" select="@Override='true'"/>
		<plx:Class name="{$ClassName}" visibility="Public" partial="true">
			<plx:ImplementsInterface dataTypeName="IORMCustomSerializedElement"/>
			<plx:Property visibility="Protected" name="SupportedCustomSerializedOperations" shadow="{$ClassOverride}">
				<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="SupportedCustomSerializedOperations"/>
				<plx:Param name="" type="RetVal" dataTypeName="ORMCustomSerializedElementSupportedOperations" dataTypeQualifier="Neumont.Tools.ORM.Shell"/>
				<plx:Get>
					<xsl:variable name="currentSupport">
						<xsl:call-template name="ReturnORMCustomSerializedElementSupportedOperations">
							<xsl:with-param name="childElements" select="count(se:ChildElement)"/>
							<xsl:with-param name="element" select="count(@Prefix)+count(@Name)+count(@Namespace)+count(@WriteStyle)+count(@DoubleTagName)+count(se:ConditionalName)"/>
							<xsl:with-param name="attributes" select="count(se:Attribute)"/>
							<xsl:with-param name="links" select="count(se:Link)"/>
							<xsl:with-param name="customSort" select="@SortChildElements='true'"/>
							<xsl:with-param name="mixedTypedAttributes" select="@HasMixedTypedAttributes='true'"/>
						</xsl:call-template>
					</xsl:variable>
					<plx:Return>
						<xsl:choose>
							<xsl:when test="$ClassOverride">
								<plx:Operator type="BitwiseOr">
									<plx:Left>
										<plx:CallInstance name="SupportedCustomSerializedOperations" type="Property">
											<plx:CallObject>
												<plx:BaseKeyword/>
											</plx:CallObject>
										</plx:CallInstance>
									</plx:Left>
									<plx:Right>
										<xsl:copy-of select="$currentSupport"/>
									</plx:Right>
								</plx:Operator>
							</xsl:when>
							<xsl:otherwise>
								<xsl:copy-of select="$currentSupport"/>
							</xsl:otherwise>
						</xsl:choose>
					</plx:Return>
				</plx:Get>
			</plx:Property>
			<xsl:variable name="childElementCount" select="count(se:ChildElement)"/>
			<xsl:variable name="haveCustomChildInfo" select="0!=$childElementCount"/>
			<xsl:if test="$haveCustomChildInfo">
				<plx:Field visibility="Private" static="true" dataTypeQualifier="Neumont.Tools.ORM.Shell" dataTypeName="ORMCustomSerializedChildElementInfo[]" name="myCustomSerializedChildElementInfo"/>
			</xsl:if>
			<xsl:if test="$haveCustomChildInfo or not($ClassOverride)">
				<plx:Function visibility="Protected" name="GetCustomSerializedChildElementInfo" shadow="{$ClassOverride}">
					<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="GetCustomSerializedChildElementInfo"/>
					<plx:Param name="" type="RetVal" dataTypeQualifier="Neumont.Tools.ORM.Shell" dataTypeName="ORMCustomSerializedChildElementInfo[]"/>
					<xsl:choose>
						<xsl:when test="$haveCustomChildInfo">
							<plx:Variable dataTypeQualifier="Neumont.Tools.ORM.Shell" dataTypeName="ORMCustomSerializedChildElementInfo" dataTypeIsSimpleArray="true" name="ret">
								<plx:Initialize>
									<plx:CallStatic dataTypeName="{$ClassName}" name="myCustomSerializedChildElementInfo" type="Field"/>
								</plx:Initialize>
							</plx:Variable>
							<plx:Condition>
								<plx:Test>
									<plx:Operator type="IdentityEquality">
										<plx:Left>
											<plx:Value type="Local" data="ret"/>
										</plx:Left>
										<plx:Right>
											<plx:NullObjectKeyword/>
										</plx:Right>
									</plx:Operator>
								</plx:Test>
								<plx:Body>
									<xsl:if test="$ClassOverride">
										<plx:Variable name="baseInfo" dataTypeName="ORMCustomSerializedChildElementInfo" dataTypeIsSimpleArray="true">
											<plx:Initialize>
												<plx:NullObjectKeyword/>
											</plx:Initialize>
										</plx:Variable>
										<plx:Variable name="baseInfoCount" dataTypeName="Int32" dataTypeQualifier="System">
											<plx:Initialize>
												<plx:Value type="I4" data="0"/>
											</plx:Initialize>
										</plx:Variable>
										<plx:Condition>
											<plx:Test>
												<plx:Operator type="Inequality">
													<plx:Left>
														<plx:Value type="I4" data="0"/>
													</plx:Left>
													<plx:Right>
														<plx:Operator type="BitwiseAnd">
															<plx:Left>
																<plx:CallStatic name="ChildElementInfo" dataTypeName="ORMCustomSerializedElementSupportedOperations" type="Field"/>
															</plx:Left>
															<plx:Right>
																<plx:CallInstance name="SupportedCustomSerializedOperations" type="Property">
																	<plx:CallObject>
																		<plx:BaseKeyword/>
																	</plx:CallObject>
																</plx:CallInstance>
															</plx:Right>
														</plx:Operator>
													</plx:Right>
												</plx:Operator>
											</plx:Test>
											<plx:Body>
												<plx:Operator type="Assign">
													<plx:Left>
														<plx:Value type="Local" data="baseInfo"/>
													</plx:Left>
													<plx:Right>
														<plx:CallInstance name="GetCustomSerializedChildElementInfo">
															<plx:CallObject>
																<plx:BaseKeyword/>
															</plx:CallObject>
														</plx:CallInstance>
													</plx:Right>
												</plx:Operator>
												<plx:Condition>
													<plx:Test>
														<plx:Operator type="IdentityInequality">
															<plx:Left>
																<plx:Value type="Local" data="baseInfo"/>
															</plx:Left>
															<plx:Right>
																<plx:NullObjectKeyword/>
															</plx:Right>
														</plx:Operator>
													</plx:Test>
													<plx:Body>
														<plx:Operator type="Assign">
															<plx:Left>
																<plx:Value type="Local" data="baseInfoCount"/>
															</plx:Left>
															<plx:Right>
																<plx:CallInstance name="Length" type="Property">
																	<plx:CallObject>
																		<plx:Value type="Local" data="baseInfo"/>
																	</plx:CallObject>
																</plx:CallInstance>
															</plx:Right>
														</plx:Operator>
													</plx:Body>
												</plx:Condition>
											</plx:Body>
										</plx:Condition>
									</xsl:if>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:Value type="Local" data="ret"/>
										</plx:Left>
										<plx:Right>
											<plx:CallNew type="New" dataTypeName="ORMCustomSerializedChildElementInfo" dataTypeQualifier="Neumont.Tools.ORM.Shell" dataTypeIsSimpleArray="true">
												<plx:PassParam>
													<xsl:choose>
														<xsl:when test="$ClassOverride">
															<plx:Operator type="Add">
																<plx:Left>
																	<plx:Value type="Local" data="baseInfoCount"/>
																</plx:Left>
																<plx:Right>
																	<plx:Value type="I4" data="{$childElementCount}"/>
																</plx:Right>
															</plx:Operator>
														</xsl:when>
														<xsl:otherwise>
															<plx:Value type="I4" data="{$childElementCount}"/>
														</xsl:otherwise>
													</xsl:choose>
												</plx:PassParam>
											</plx:CallNew>
										</plx:Right>
									</plx:Operator>
									<xsl:if test="$ClassOverride">
										<plx:Condition>
											<plx:Test>
												<plx:Operator type="BooleanNot">
													<!-- UNDONE: Plix CodeDom game -->
													<plx:Operator type="Equality">
														<plx:Left>
															<plx:Value type="Local" data="baseInfoCount"/>
														</plx:Left>
														<plx:Right>
															<plx:Value type="I4" data="0"/>
														</plx:Right>
													</plx:Operator>
												</plx:Operator>
											</plx:Test>
											<plx:Body>
												<plx:CallInstance name="CopyTo">
													<plx:CallObject>
														<plx:Value type="Local" data="baseInfo"/>
													</plx:CallObject>
													<plx:PassParam>
														<plx:Value type="Local" data="ret"/>
													</plx:PassParam>
													<plx:PassParam>
														<plx:Value type="I4" data="{$childElementCount}"/>
													</plx:PassParam>
												</plx:CallInstance>
											</plx:Body>
										</plx:Condition>
									</xsl:if>
									<xsl:for-each select="se:ChildElement">
										<xsl:variable name="index" select="position()-1"/>
										<xsl:call-template name="CreateORMCustomSerializedElementInfoNameVariable">
											<xsl:with-param name="modifier" select="$index"/>
										</xsl:call-template>
										<plx:Operator type="Assign">
											<plx:Left>
												<plx:CallInstance name="" type="ArrayIndexer">
													<plx:CallObject>
														<plx:Value type="Local" data="ret"/>
													</plx:CallObject>
													<plx:PassParam>
														<plx:Value type="Local" data="{$index}"/>
													</plx:PassParam>
												</plx:CallInstance>
											</plx:Left>
											<plx:Right>
												<plx:CallNew type="New" dataTypeQualifier="Neumont.Tools.ORM.Shell" dataTypeName="ORMCustomSerializedChildElementInfo">
													<xsl:call-template name="PassORMCustomSerializedElementInfoParams">
														<xsl:with-param name="modifier" select="$index"/>
													</xsl:call-template>
													<xsl:for-each select="se:Link">
														<plx:PassParam>
															<plx:CallStatic name="{@RoleName}MetaRoleGuid" dataTypeName="{@RelationshipName}" type="Field"/>
														</plx:PassParam>
													</xsl:for-each>
												</plx:CallNew>
											</plx:Right>
										</plx:Operator>
									</xsl:for-each>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:CallStatic dataTypeName="{$ClassName}" name="myCustomSerializedChildElementInfo" type="Field"/>
										</plx:Left>
										<plx:Right>
											<plx:Value type="Local" data="ret"/>
										</plx:Right>
									</plx:Operator>
								</plx:Body>
							</plx:Condition>
							<plx:Return>
								<plx:Value type="Local" data="ret"/>
							</plx:Return>
						</xsl:when>
						<xsl:otherwise>
							<plx:Throw>
								<plx:CallNew type="New" dataTypeQualifier="System" dataTypeName="NotSupportedException"/>
							</plx:Throw>
						</xsl:otherwise>
					</xsl:choose>
				</plx:Function>
			</xsl:if>
			<xsl:variable name="haveCustomElementInfo" select="0!=(string-length(@Prefix)+string-length(@Name)+string-length(@Namespace)+string-length(@WriteStyle)+string-length(@DoubleTagName)+count(se:ConditionalName))"/>
			<xsl:if test="$haveCustomElementInfo or not($ClassOverride)">
				<plx:Property visibility="Protected" name="CustomSerializedElementInfo" shadow="{$ClassOverride}">
					<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="CustomSerializedElementInfo"/>
					<plx:Param name="" type="RetVal" dataTypeName="ORMCustomSerializedElementInfo" dataTypeQualifier="Neumont.Tools.ORM.Shell"/>
					<plx:Get>
						<xsl:choose>
							<xsl:when test="$haveCustomElementInfo">
								<xsl:call-template name="ReturnORMCustomSerializedElementInfo"/>
							</xsl:when>
							<xsl:otherwise>
								<plx:Throw>
									<plx:CallNew type="New" dataTypeQualifier="System" dataTypeName="NotSupportedException"/>
								</plx:Throw>
							</xsl:otherwise>
						</xsl:choose>
					</plx:Get>
				</plx:Property>
			</xsl:if>
			<xsl:variable name="haveCustomAttributeInfo" select="0!=count(se:Attribute)"/>
			<xsl:if test="$haveCustomAttributeInfo or not($ClassOverride)">
				<plx:Function visibility="Protected" name="GetCustomSerializedAttributeInfo" shadow="{$ClassOverride}">
					<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="GetCustomSerializedAttributeInfo"/>
					<plx:Param name="" type="RetVal" dataTypeName="ORMCustomSerializedAttributeInfo" dataTypeQualifier="Neumont.Tools.ORM.Shell"/>
					<plx:Param name="attributeInfo" dataTypeName="MetaAttributeInfo" dataTypeQualifier="Microsoft.VisualStudio.Modeling"></plx:Param>
					<plx:Param name="rolePlayedInfo" dataTypeName="MetaRoleInfo" dataTypeQualifier="Microsoft.VisualStudio.Modeling"></plx:Param>
					<xsl:choose>
						<xsl:when test="count(se:Attribute)">
							<xsl:for-each select="se:Attribute">
								<plx:Condition>
									<plx:Test>
										<plx:Operator type="Equality">
											<plx:Left>
												<plx:CallInstance type="Property" name="Id">
													<plx:CallObject>
														<plx:Value type="Parameter" data="attributeInfo"/>
													</plx:CallObject>
												</plx:CallInstance>
											</plx:Left>
											<plx:Right>
												<plx:CallStatic type="Field" name="{@ID}MetaAttributeGuid" dataTypeName="{$ClassName}" />
											</plx:Right>
										</plx:Operator>
									</plx:Test>
									<plx:Body>
										<xsl:for-each select="se:RolePlayed">
											<plx:Condition>
												<plx:Test>
													<plx:Operator type="Equality">
														<plx:Left>
															<plx:CallInstance type="Property" name="Id">
																<plx:CallObject>
																	<plx:Value type="Parameter" data="rolePlayedInfo"/>
																</plx:CallObject>
															</plx:CallInstance>
														</plx:Left>
														<plx:Right>
															<plx:CallStatic type="Field" name="{@ID}MetaRoleGuid" dataTypeName="{$ClassName}" />
														</plx:Right>
													</plx:Operator>
												</plx:Test>
												<plx:Body>
													<xsl:call-template name="ReturnORMCustomSerializedAttributeInfo"/>
												</plx:Body>
											</plx:Condition>
										</xsl:for-each>
										<xsl:call-template name="ReturnORMCustomSerializedAttributeInfo"/>
									</plx:Body>
								</plx:Condition>
							</xsl:for-each>
							<xsl:if test="$ClassOverride">
								<plx:Condition>
									<plx:Test>
										<plx:Operator type="Inequality">
											<plx:Left>
												<plx:Value type="I4" data="0"/>
											</plx:Left>
											<plx:Right>
												<plx:Operator type="BitwiseAnd">
													<plx:Left>
														<plx:CallStatic name="AttributeInfo" dataTypeName="ORMCustomSerializedElementSupportedOperations" type="Field"/>
													</plx:Left>
													<plx:Right>
														<plx:CallInstance name="SupportedCustomSerializedOperations" type="Property">
															<plx:CallObject>
																<plx:BaseKeyword/>
															</plx:CallObject>
														</plx:CallInstance>
													</plx:Right>
												</plx:Operator>
											</plx:Right>
										</plx:Operator>
									</plx:Test>
									<plx:Body>
										<plx:Return>
											<plx:CallInstance name="GetCustomSerializedAttributeInfo">
												<plx:CallObject>
													<plx:BaseKeyword/>
												</plx:CallObject>
												<plx:PassParam>
													<plx:Value type="Parameter" data="attributeInfo"/>
												</plx:PassParam>
												<plx:PassParam>
													<plx:Value type="Parameter" data="rolePlayedInfo"/>
												</plx:PassParam>
											</plx:CallInstance>
										</plx:Return>
									</plx:Body>
								</plx:Condition>
							</xsl:if>
							<plx:Return>
								<plx:CallStatic dataTypeName="ORMCustomSerializedAttributeInfo" name="Default" type="Field"/>
							</plx:Return>
						</xsl:when>
						<xsl:otherwise>
							<plx:Throw>
								<plx:CallNew type="New" dataTypeQualifier="System" dataTypeName="NotSupportedException"/>
							</plx:Throw>
						</xsl:otherwise>
					</xsl:choose>
				</plx:Function>
			</xsl:if>
			<xsl:variable name="haveCustomLinkInfo" select="0!=count(se:Link)"/>
			<xsl:if test="$haveCustomLinkInfo or not($ClassOverride)">
				<plx:Function visibility="Protected" name="GetCustomSerializedLinkInfo" shadow="{$ClassOverride}">
					<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="GetCustomSerializedLinkInfo"/>
					<plx:Param name="" type="RetVal" dataTypeName="ORMCustomSerializedElementInfo" dataTypeQualifier="Neumont.Tools.ORM.Shell"/>
					<plx:Param name="rolePlayedInfo" dataTypeName="MetaRoleInfo" dataTypeQualifier="Microsoft.VisualStudio.Modeling"></plx:Param>
					<xsl:choose>
						<xsl:when test="$haveCustomLinkInfo">
							<xsl:for-each select="se:Link">
								<plx:Condition>
									<plx:Test>
										<plx:Operator type="Equality">
											<plx:Left>
												<plx:CallInstance type="Property" name="Id">
													<plx:CallObject>
														<plx:Value type="Parameter" data="rolePlayedInfo"/>
													</plx:CallObject>
												</plx:CallInstance>
											</plx:Left>
											<plx:Right>
												<plx:CallStatic type="Field" name="{@RoleName}MetaRoleGuid" dataTypeName="{@RelationshipName}" />
											</plx:Right>
										</plx:Operator>
									</plx:Test>
									<plx:Body>
										<xsl:call-template name="ReturnORMCustomSerializedElementInfo"/>
									</plx:Body>
								</plx:Condition>
							</xsl:for-each>
							<xsl:if test="$ClassOverride">
								<plx:Condition>
									<plx:Test>
										<plx:Operator type="Inequality">
											<plx:Left>
												<plx:Value type="I4" data="0"/>
											</plx:Left>
											<plx:Right>
												<plx:Operator type="BitwiseAnd">
													<plx:Left>
														<plx:CallStatic name="LinkInfo" dataTypeName="ORMCustomSerializedElementSupportedOperations" type="Field"/>
													</plx:Left>
													<plx:Right>
														<plx:CallInstance name="SupportedCustomSerializedOperations" type="Property">
															<plx:CallObject>
																<plx:BaseKeyword/>
															</plx:CallObject>
														</plx:CallInstance>
													</plx:Right>
												</plx:Operator>
											</plx:Right>
										</plx:Operator>
									</plx:Test>
									<plx:Body>
										<plx:Return>
											<plx:CallInstance name="GetCustomSerializedLinkInfo">
												<plx:CallObject>
													<plx:BaseKeyword/>
												</plx:CallObject>
												<plx:PassParam>
													<plx:Value type="Parameter" data="rolePlayedInfo"/>
												</plx:PassParam>
											</plx:CallInstance>
										</plx:Return>
									</plx:Body>
								</plx:Condition>
							</xsl:if>
							<plx:Return>
								<plx:CallStatic dataTypeName="ORMCustomSerializedElementInfo" name="Default" type="Field"/>
							</plx:Return>
						</xsl:when>
						<xsl:otherwise>
							<plx:Throw>
								<plx:CallNew type="New" dataTypeQualifier="System" dataTypeName="NotSupportedException"/>
							</plx:Throw>
						</xsl:otherwise>
					</xsl:choose>
				</plx:Function>
			</xsl:if>
			<xsl:choose>
				<xsl:when test="@SortChildElements='true'">
					<plx:Field name="myCustomSortChildComparer" static="true" visibility="Private" dataTypeName="IComparer">
						<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
					</plx:Field>
					<plx:Class name="CustomSortChildComparer" visibility="Private">
						<plx:ImplementsInterface dataTypeName="IComparer">
							<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
						</plx:ImplementsInterface>
						<plx:Field name="myRoleOrderDictionary" visibility="Private" dataTypeName="Dictionary">
							<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
							<plx:PassTypeParam dataTypeName="Int32" dataTypeQualifier="System"/>
						</plx:Field>
						<xsl:if test="$ClassOverride">
							<plx:Field name="myBaseComparer" visibility="Private" dataTypeName="IComparer">
								<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
							</plx:Field>
						</xsl:if>
						<plx:Function ctor="true" name="" visibility="Public">
							<plx:Param name="store" dataTypeName="Store" type="In"/>
							<xsl:if test="$ClassOverride">
								<plx:Param name="baseComparer" dataTypeName="IComparer">
									<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
								</plx:Param>
								<plx:Operator type="Assign">
									<plx:Left>
										<plx:CallInstance name="myBaseComparer" type="Field">
											<plx:CallObject>
												<plx:ThisKeyword/>
											</plx:CallObject>
										</plx:CallInstance>
									</plx:Left>
									<plx:Right>
										<plx:Value type="Parameter" data="baseComparer"/>
									</plx:Right>
								</plx:Operator>
							</xsl:if>
							<xsl:variable name="SortedLevelsFragment">
								<!-- ChildElement/Link links may have more information in Link. Just use
								     the ChildElement one. -->
								<xsl:variable name="childLinks" select="se:ChildElement[not(@NotSorted='true')]/se:Link"/>
								<!-- Define a variable with structure <SortLevel><Role/><SortLevel/> -->
								<xsl:for-each select="se:Link | se:ChildElement">
									<xsl:if test="not(@NotSorted='true')">
										<xsl:choose>
											<xsl:when test="local-name()='Link'">
												<xsl:variable name="relName" select="@RelationshipName"/>
												<xsl:variable name="roleName" select="@RoleName"/>
												<xsl:if test="0=count($childLinks[@RelationshipName=$relName and @RoleName=$roleName])">
													<SortLevel>
														<Role RelationshipName="{@RelationshipName}" RoleName="{@RoleName}"/>
													</SortLevel>
												</xsl:if>
											</xsl:when>
											<xsl:when test="local-name()='ChildElement'">
												<xsl:choose>
													<xsl:when test="@SortChildElements='true'">
														<!-- Add one sort level for each child -->
														<xsl:for-each select="se:Link">
															<SortLevel>
																<Role RelationshipName="{@RelationshipName}" RoleName="{@RoleName}"/>
															</SortLevel>
														</xsl:for-each>
													</xsl:when>
													<xsl:otherwise>
														<!-- Add one sort level for all children -->
														<SortLevel>
															<xsl:for-each select="se:Link">
																<Role RelationshipName="{@RelationshipName}" RoleName="{@RoleName}"/>
															</xsl:for-each>
														</SortLevel>
													</xsl:otherwise>
												</xsl:choose>
											</xsl:when>
										</xsl:choose>
									</xsl:if>
								</xsl:for-each>
							</xsl:variable>
							<xsl:variable name="SortedLevels" select="msxsl:node-set($SortedLevelsFragment)"/>
							<plx:Variable name="metaDataDir" dataTypeName="MetaDataDirectory">
								<plx:Initialize>
									<plx:CallInstance name="MetaDataDirectory" type="Property">
										<plx:CallObject>
											<plx:Value type="Parameter" data="store"/>
										</plx:CallObject>
									</plx:CallInstance>
								</plx:Initialize>
							</plx:Variable>
							<plx:Variable name="roleOrderDictionary" dataTypeName="Dictionary">
								<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
								<plx:PassTypeParam dataTypeName="Int32" dataTypeQualifier="System"/>
								<plx:Initialize>
									<plx:CallNew dataTypeName="Dictionary">
										<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
										<plx:PassTypeParam dataTypeName="Int32" dataTypeQualifier="System"/>
									</plx:CallNew>
								</plx:Initialize>
							</plx:Variable>
							<plx:Variable name="metaRole" dataTypeName="MetaRoleInfo"/>
							<xsl:for-each select="$SortedLevels/SortLevel">
								<xsl:variable name="level" select="position()-1"/>
								<xsl:for-each select="Role">
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:Value type="Local" data="metaRole"/>
										</plx:Left>
										<plx:Right>
											<plx:CallInstance name="FindMetaRole">
												<plx:CallObject>
													<plx:Value type="Local" data="metaDataDir"/>
												</plx:CallObject>
												<plx:PassParam>
													<plx:CallStatic dataTypeName="{@RelationshipName}" name="{@RoleName}MetaRoleGuid" type="Field"/>
												</plx:PassParam>
											</plx:CallInstance>
										</plx:Right>
									</plx:Operator>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:CallInstance name="" type="Indexer">
												<plx:CallObject>
													<plx:Value type="Local" data="roleOrderDictionary"/>
												</plx:CallObject>
												<plx:PassParam>
													<plx:CallInstance name="FullName" type="Property">
														<plx:CallObject>
															<plx:CallInstance name="OppositeMetaRole" type="Property">
																<plx:CallObject>
																	<plx:Value type="Local" data="metaRole"/>
																</plx:CallObject>
															</plx:CallInstance>
														</plx:CallObject>
													</plx:CallInstance>
												</plx:PassParam>
											</plx:CallInstance>
										</plx:Left>
										<plx:Right>
											<plx:Value type="I4" data="{$level}"/>
										</plx:Right>
									</plx:Operator>
								</xsl:for-each>
							</xsl:for-each>
							<plx:Operator type="Assign">
								<plx:Left>
									<plx:CallInstance name="myRoleOrderDictionary" type="Field">
										<plx:CallObject>
											<plx:ThisKeyword/>
										</plx:CallObject>
									</plx:CallInstance>
								</plx:Left>
								<plx:Right>
									<plx:Value type="Local" data="roleOrderDictionary"/>
								</plx:Right>
							</plx:Operator>
						</plx:Function>
						<plx:Function visibility="Public" name="Compare">
							<!-- UNDONE: I'd prefer the following block, but Beta2 CodeDom isn't
								spitting type arguments for private implementation types
							<plx:InterfaceMember dataTypeName="IComparer" member="Compare">
								<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
							</plx:InterfaceMember>-->
							<plx:Param type="RetVal" name="" dataTypeName="Int32" dataTypeQualifier="System"/>
							<plx:Param type="In" name="x" dataTypeName="MetaRoleInfo"/>
							<plx:Param type="In" name="y" dataTypeName="MetaRoleInfo"/>
							<xsl:if test="$ClassOverride">
								<!-- Give the base the first shot, we want base elements displayed before derived elements -->
								<plx:Condition>
									<plx:Test>
										<plx:Operator type="IdentityInequality">
											<plx:Left>
												<plx:CallInstance name="myBaseComparer" type="Field">
													<plx:CallObject>
														<plx:ThisKeyword/>
													</plx:CallObject>
												</plx:CallInstance>
											</plx:Left>
											<plx:Right>
												<plx:NullObjectKeyword/>
											</plx:Right>
										</plx:Operator>
									</plx:Test>
									<plx:Body>
										<plx:Variable name="baseOpinion" dataTypeName="Int32" dataTypeQualifier="System">
											<plx:Initialize>
												<plx:CallInstance name="Compare">
													<plx:CallObject>
														<plx:CallInstance name="myBaseComparer" type="Field">
															<plx:CallObject>
																<plx:ThisKeyword/>
															</plx:CallObject>
														</plx:CallInstance>
													</plx:CallObject>
													<plx:PassParam>
														<plx:Value type="Parameter" data="x"/>
													</plx:PassParam>
													<plx:PassParam>
														<plx:Value type="Parameter" data="y"/>
													</plx:PassParam>
												</plx:CallInstance>
											</plx:Initialize>
										</plx:Variable>
										<plx:Condition>
											<plx:Test>
												<plx:Operator type="Inequality">
													<plx:Left>
														<plx:Value type="I4" data="0"/>
													</plx:Left>
													<plx:Right>
														<plx:Value type="Local" data="baseOpinion"/>
													</plx:Right>
												</plx:Operator>
											</plx:Test>
											<plx:Body>
												<plx:Return>
													<plx:Value type="Local" data="baseOpinion"/>
												</plx:Return>
											</plx:Body>
										</plx:Condition>
									</plx:Body>
								</plx:Condition>
							</xsl:if>
							<xsl:variable name="paramVals">
								<Value>x</Value>
								<Value>y</Value>
							</xsl:variable>
							<xsl:for-each select="msxsl:node-set($paramVals)/child::*">
								<plx:Variable name="{.}Pos" dataTypeName="Int32" dataTypeQualifier="System"/>
								<plx:Condition>
									<plx:Test>
										<plx:Operator type="BooleanNot">
											<plx:CallInstance name="TryGetValue">
												<plx:CallObject>
													<plx:CallInstance name="myRoleOrderDictionary" type="Field">
														<plx:CallObject>
															<plx:ThisKeyword/>
														</plx:CallObject>
													</plx:CallInstance>
												</plx:CallObject>
												<plx:PassParam passStyle="In">
													<plx:CallInstance name="FullName" type="Property">
														<plx:CallObject>
															<plx:Value type="Parameter" data="{.}"/>
														</plx:CallObject>
													</plx:CallInstance>
												</plx:PassParam>
												<plx:PassParam passStyle="Out">
													<plx:Value type="Local" data="{.}Pos"/>
												</plx:PassParam>
											</plx:CallInstance>
										</plx:Operator>
									</plx:Test>
									<plx:Body>
										<plx:Operator type="Assign">
											<plx:Left>
												<plx:Value type="Local" data="{.}Pos"/>
											</plx:Left>
											<plx:Right>
												<plx:CallStatic dataTypeName="Int32" dataTypeQualifier="System" name="MaxValue" type="Field"/>
											</plx:Right>
										</plx:Operator>
									</plx:Body>
								</plx:Condition>
							</xsl:for-each>
							<plx:Condition>
								<plx:Test>
									<plx:Operator type="Equality">
										<plx:Left>
											<plx:Value type="Local" data="xPos"/>
										</plx:Left>
										<plx:Right>
											<plx:Value type="Local"  data="yPos"/>
										</plx:Right>
									</plx:Operator>
								</plx:Test>
								<plx:Body>
									<plx:Return>
										<plx:Value type="I4" data="0"/>
									</plx:Return>
								</plx:Body>
								<plx:FallbackCondition>
									<plx:Test>
										<plx:Operator type="LessThan">
											<plx:Left>
												<plx:Value type="Local" data="xPos"/>
											</plx:Left>
											<plx:Right>
												<plx:Value type="Local" data="yPos"/>
											</plx:Right>
										</plx:Operator>
									</plx:Test>
									<plx:Body>
										<plx:Return>
											<plx:Value type="I4" data="-1"/>
										</plx:Return>
									</plx:Body>
								</plx:FallbackCondition>
							</plx:Condition>
							<plx:Return>
								<plx:Value type="I4" data="1"/>
							</plx:Return>
						</plx:Function>
					</plx:Class>
					<plx:Property visibility="Protected" name="CustomSerializedChildRoleComparer" shadow="{$ClassOverride}">
						<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="CustomSerializedChildRoleComparer"/>
						<plx:Attribute dataTypeName="CLSCompliant">
							<plx:PassParam>
								<plx:FalseKeyword/>
							</plx:PassParam>
						</plx:Attribute>
						<plx:Param dataTypeName="IComparer" type="RetVal" name="">
							<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
						</plx:Param>
						<plx:Get>
							<plx:Variable name="retVal" dataTypeName="IComparer">
								<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
								<plx:Initialize>
									<plx:CallStatic dataTypeName="{$ClassName}" name="myCustomSortChildComparer" type="Field"/>
								</plx:Initialize>
							</plx:Variable>
							<plx:Condition>
								<plx:Test>
									<plx:Operator type="IdentityEquality">
										<plx:Left>
											<plx:NullObjectKeyword/>
										</plx:Left>
										<plx:Right>
											<plx:Value type="Local" data="retVal"/>
										</plx:Right>
									</plx:Operator>
								</plx:Test>
								<plx:Body>
									<xsl:if test="$ClassOverride">
										<plx:Variable name="baseComparer" dataTypeName="IComparer">
											<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
											<plx:Initialize>
												<plx:NullObjectKeyword/>
											</plx:Initialize>
										</plx:Variable>
										<plx:Condition>
											<plx:Test>
												<plx:Operator type="Inequality">
													<plx:Left>
														<plx:Value type="I4" data="0"/>
													</plx:Left>
													<plx:Right>
														<plx:Operator type="BitwiseAnd">
															<plx:Left>
																<plx:CallStatic name="CustomSortChildRoles" dataTypeName="ORMCustomSerializedElementSupportedOperations" type="Field"/>
															</plx:Left>
															<plx:Right>
																<plx:CallInstance name="SupportedCustomSerializedOperations" type="Property">
																	<plx:CallObject>
																		<plx:BaseKeyword/>
																	</plx:CallObject>
																</plx:CallInstance>
															</plx:Right>
														</plx:Operator>
													</plx:Right>
												</plx:Operator>
											</plx:Test>
											<plx:Body>
												<plx:Operator type="Assign">
													<plx:Left>
														<plx:Value type="Local" data="baseComparer"/>
													</plx:Left>
													<plx:Right>
														<plx:CallInstance name="CustomSerializedChildRoleComparer" type="Property">
															<plx:CallObject>
																<plx:BaseKeyword/>
															</plx:CallObject>
														</plx:CallInstance>
													</plx:Right>
												</plx:Operator>
											</plx:Body>
										</plx:Condition>
									</xsl:if>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:Value type="Local" data="retVal"/>
										</plx:Left>
										<plx:Right>
											<plx:CallNew dataTypeName="CustomSortChildComparer">
												<plx:PassParam>
													<plx:CallInstance name="Store" type="Property">
														<plx:CallObject>
															<plx:ThisKeyword/>
														</plx:CallObject>
													</plx:CallInstance>
												</plx:PassParam>
												<xsl:if test="$ClassOverride">
													<plx:PassParam>
														<plx:Value type="Local" data="baseComparer"/>
													</plx:PassParam>
												</xsl:if>
											</plx:CallNew>
										</plx:Right>
									</plx:Operator>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:CallStatic dataTypeName="{$ClassName}" name="myCustomSortChildComparer" type="Field"/>
										</plx:Left>
										<plx:Right>
											<plx:Value type="Local" data="retVal"/>
										</plx:Right>
									</plx:Operator>
								</plx:Body>
							</plx:Condition>
							<plx:Return>
								<plx:Value type="Local" data="retVal"/>
							</plx:Return>
						</plx:Get>
					</plx:Property>
				</xsl:when>
				<xsl:when test="not($ClassOverride)">
					<plx:Property visibility="Protected" name="CustomSerializedChildRoleComparer">
						<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="CustomSerializedChildRoleComparer"/>
						<plx:Attribute dataTypeName="CLSCompliant">
							<plx:PassParam>
								<plx:FalseKeyword/>
							</plx:PassParam>
						</plx:Attribute>
						<plx:Param dataTypeName="IComparer" type="RetVal" name="">
							<plx:PassTypeParam dataTypeName="MetaRoleInfo"/>
						</plx:Param>
						<plx:Get>
							<plx:Return>
								<plx:NullObjectKeyword/>
							</plx:Return>
						</plx:Get>
					</plx:Property>
				</xsl:when>
			</xsl:choose>
			<xsl:variable name="mapChildElementBodyFragment">
				<xsl:variable name="namespaces" select="../se:MetaModel/se:Namespaces/se:Namespace"/>
				<xsl:variable name="namespace">
					<xsl:call-template name="ResolveNamespace">
						<xsl:with-param name="namespaces" select="$namespaces"/>
						<!-- Use default for prefix parameter -->
					</xsl:call-template>
				</xsl:variable>
				<xsl:variable name="linksInChildElementFragment">
					<xsl:for-each select="se:ChildElement/se:Link">
						<xsl:copy>
							<xsl:copy-of select="@*"/>
							<xsl:for-each select="parent::se:ChildElement">
								<xsl:attribute name="ContainerName">
									<xsl:value-of select="@Name"/>
								</xsl:attribute>
								<xsl:attribute name="ContainerPrefix">
									<xsl:value-of select="@Prefix"/>
								</xsl:attribute>
							</xsl:for-each>
						</xsl:copy>
					</xsl:for-each>
				</xsl:variable>
				<xsl:variable name="linksInChildElement" select="msxsl:node-set($linksInChildElementFragment)/child::*"/>
				<xsl:variable name="childElements" select="se:ChildElement"/>
				<xsl:variable name="allLinksTemp">
					<xsl:for-each select="se:Link[not(@WriteStyle='NotWritten')]">
						<xsl:variable name="relationship" select="@RelationshipName"/>
						<xsl:variable name="role" select="@RoleName"/>
						<xsl:choose>
							<xsl:when test="count($linksInChildElement[@RelationshipName=$relationship and @RoleName=$role]) > 0">
								<xsl:copy>
									<xsl:copy-of select="@*"/>
									<xsl:attribute name="contained">
										<xsl:value-of select="true()"/>
									</xsl:attribute>
								</xsl:copy>
							</xsl:when>
							<xsl:otherwise>
								<xsl:copy>
									<xsl:copy-of select="@*"/>
									<xsl:attribute name="uncontained">
										<xsl:value-of select="true()"/>
									</xsl:attribute>
								</xsl:copy>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:for-each>
				</xsl:variable>
				<xsl:variable name="allLinks" select="msxsl:node-set($allLinksTemp)/child::*"/>

				<xsl:for-each select="$allLinks">
					<xsl:choose>
						<!-- Walk the $allLinks, then add the ones that are
						NOT contained in $linksInChildElement -->
						<xsl:when test="@uncontained">
							<plx:CallInstance name="InitializeRoles">
								<plx:CallObject>
									<plx:Value type="Local" data="match"/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:CallStatic name="{@RoleName}MetaRoleGuid" dataTypeName="{@RelationshipName}" type="Property"/>
								</plx:PassParam>
							</plx:CallInstance>
							<plx:CallInstance name="Add">
								<plx:CallObject>
									<plx:Value type="Local" data="childElementMappings"/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:String>
										<plx:String>||</plx:String>
										<plx:String>
											<xsl:call-template name="ResolveNamespace">
												<xsl:with-param name="namespaces" select="$namespaces"/>
												<!-- Use default for prefix parameter -->
											</xsl:call-template>
										</plx:String>
										<plx:String>|</plx:String>
										<plx:String>
											<xsl:value-of select="@Name"/>
										</plx:String>
									</plx:String>
								</plx:PassParam>
								<plx:PassParam>
									<plx:Value type="Local" data="match"/>
								</plx:PassParam>
							</plx:CallInstance>
						</xsl:when>
					</xsl:choose>
				</xsl:for-each>
				<!-- Walk $linksInChildElements, then add the ones that
				INTERSECT with the $allLinks list to the dictionary. -->
				<xsl:for-each select="$childElements">
					<xsl:variable name="localLinksFragment">
						<xsl:for-each select="se:Link">
							<xsl:variable name="relationshipName" select="@RelationshipName"/>
							<xsl:variable name="roleName" select="@RoleName"/>
							<xsl:variable name="namedLinks" select="$allLinks[@RelationshipName=$relationshipName and @RoleName=$roleName]"/>
							<xsl:if test="count($namedLinks)">
								<xsl:copy>
									<xsl:copy-of select="@*"/>
									<xsl:for-each select="$namedLinks[1]">
										<xsl:choose>
											<xsl:when test="string-length(@Name)">
												<xsl:copy-of select="@Name"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:attribute name="Name">
													<xsl:value-of select="@RelationshipName"/>
													<xsl:text>.</xsl:text>
													<xsl:value-of select="@RoleName"/>
												</xsl:attribute>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:for-each>
								</xsl:copy>
							</xsl:if>
						</xsl:for-each>
					</xsl:variable>
					<xsl:variable name="localLinks" select="msxsl:node-set($localLinksFragment)/child::*"/>
					<xsl:if test="count($localLinks)">
						<xsl:variable name="containerName" select="@Name"/>
						<xsl:for-each select="$localLinks">
							<plx:CallInstance name="InitializeRoles">
								<plx:CallObject>
									<plx:Value type="Local" data="match"/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:CallStatic name="{@RoleName}MetaRoleGuid" dataTypeName="{@RelationshipName}" type="Field"/>
								</plx:PassParam>
							</plx:CallInstance>
							<plx:CallInstance name="Add">
								<plx:CallObject>
									<plx:Value type="Local" data="childElementMappings"/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:String>
										<plx:String>
											<xsl:call-template name="ResolveNamespace">
												<xsl:with-param name="namespaces" select="$namespaces"/>
												<!-- Use default for prefix parameter -->
											</xsl:call-template>
										</plx:String>
										<plx:String>|</plx:String>
										<plx:String>
											<plx:String>
												<xsl:value-of select="$containerName"/>
											</plx:String>
										</plx:String>
										<plx:String>|</plx:String>
										<plx:String>
											<xsl:call-template name="ResolveNamespace">
												<xsl:with-param name="namespaces" select="$namespaces"/>
												<!-- Use default for prefix parameter -->
											</xsl:call-template>
										</plx:String>
										<plx:String>|</plx:String>
										<plx:String>
											<xsl:value-of select="@Name"/>
										</plx:String>
									</plx:String>
								</plx:PassParam>
								<plx:PassParam>
									<plx:Value type="Local" data="match"/>
								</plx:PassParam>
							</plx:CallInstance>
						</xsl:for-each>
					</xsl:if>
				</xsl:for-each>

				<!-- Walk $linksInChildElements, then add the ones that are
				NOT in the $allLinks list to the dictionary. -->
				<xsl:for-each select="$childElements">
					<xsl:variable name="linksFragment">
						<xsl:for-each select="se:Link">
							<xsl:variable name="relationshipName" select="@RelationshipName"/>
							<xsl:variable name="roleName" select="@RoleName"/>
							<xsl:variable name="containerName" select="@ContainerName"/>
							<xsl:variable name="containerPrefix" select="@ContainerPrefix"/>
							<xsl:if test="count($allLinks[@RelationshipName=$relationshipName and @RoleName=$roleName]) = 0">
								<plx:PassParam>
									<plx:CallStatic name="{@RoleName}MetaRoleGuid" dataTypeName="{@RelationshipName}" type="Property"/>
								</plx:PassParam>
							</xsl:if>
						</xsl:for-each>
					</xsl:variable>
					<xsl:variable name="links" select="msxsl:node-set($linksFragment)/child::*"/>
					<xsl:if test="count($links)">
						<plx:CallInstance name="InitializeRoles">
							<plx:CallObject>
								<plx:Value type="Local" data="match"/>
							</plx:CallObject>
							<xsl:copy-of select="$links"/>
						</plx:CallInstance>
						<plx:CallInstance name="Add">
							<plx:CallObject>
								<plx:Value type="Local" data="childElementMappings"/>
							</plx:CallObject>
							<plx:PassParam>
								<plx:String>
									<plx:String>
										<xsl:call-template name="ResolveNamespace">
											<xsl:with-param name="namespaces" select="$namespaces"/>
											<!-- Use default for prefix parameter -->
										</xsl:call-template>
									</plx:String>
									<plx:String>|</plx:String>
									<plx:String>
										<plx:String>
											<xsl:value-of select="@Name"/>
										</plx:String>
									</plx:String>
									<plx:String>||</plx:String>
								</plx:String>
							</plx:PassParam>
							<plx:PassParam>
								<plx:Value type="Local" data="match"/>
							</plx:PassParam>
						</plx:CallInstance>
					</xsl:if>
				</xsl:for-each>


				<xsl:for-each select="se:ChildElement">
					<xsl:choose>
						<!-- It's a relationship -->
						<xsl:when test="se:Link">
							<xsl:for-each select="se:Link">
								<!--										<xsl:if test="@RelationshipName = parent::parent::se:Element/se:Link"></xsl:if>-->
							</xsl:for-each>
						</xsl:when>
						<!-- It's a something that I don't find an example for-->
						<xsl:otherwise>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>

				<!-- Add reverse mapping for attributes serialized as elements -->
				<xsl:for-each select="se:Attribute[@WriteStyle='Element' or @WriteStyle='DoubleTaggedElement']">
					<plx:CallInstance name="InitializeAttribute">
						<plx:CallObject>
							<plx:Value type="Local" data="match"/>
						</plx:CallObject>
						<plx:PassParam>
							<plx:CallStatic name="{@ID}MetaAttributeGuid" dataTypeName="{$ClassName}" type="Field"/>
						</plx:PassParam>
						<plx:PassParam>
							<xsl:choose>
								<xsl:when test="string-length(@DoubleTagName)">
									<plx:String>
										<xsl:value-of select="@DoubleTagName"/>
									</plx:String>
								</xsl:when>
								<xsl:otherwise>
									<plx:NullObjectKeyword/>
								</xsl:otherwise>
							</xsl:choose>
						</plx:PassParam>
					</plx:CallInstance>
					<plx:CallInstance name="Add">
						<plx:CallObject>
							<plx:Value type="Local" data="childElementMappings"/>
						</plx:CallObject>
						<plx:PassParam>
							<plx:String>
								<plx:String>||</plx:String>
								<plx:String>
									<xsl:call-template name="ResolveNamespace">
										<xsl:with-param name="namespaces" select="$namespaces"/>
										<!-- Use default for prefix parameter -->
									</xsl:call-template>
								</plx:String>
								<plx:String>|</plx:String>
								<plx:String>
									<xsl:choose>
										<xsl:when test="string-length(@Name)">
											<xsl:value-of select="@Name"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="@ID"/>
										</xsl:otherwise>
									</xsl:choose>
								</plx:String>
							</plx:String>
						</plx:PassParam>
						<plx:PassParam>
							<plx:Value type="Local" data="match"/>
						</plx:PassParam>
					</plx:CallInstance>
				</xsl:for-each>
			</xsl:variable>
			<xsl:variable name="mapChildElementBody" select="msxsl:node-set($mapChildElementBodyFragment)/child::*"/>
			<xsl:variable name="hasMappedChildElements" select="0!=count($mapChildElementBody)"/>
			<xsl:if test="$hasMappedChildElements">
				<plx:Field name="myChildElementMappings" dataTypeName="Dictionary" visibility="Private" static="true">
					<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
					<plx:PassTypeParam dataTypeName="ORMCustomSerializedElementMatch"/>
				</plx:Field>
			</xsl:if>
			<xsl:if test="$hasMappedChildElements or not($ClassOverride)">
				<plx:Function visibility="Protected" name="MapChildElement" shadow="{$ClassOverride}">
					<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="MapChildElement"/>
					<plx:Param dataTypeName="String" dataTypeQualifier="System" name="elementNamespace"/>
					<plx:Param dataTypeName="String" dataTypeQualifier="System" name="elementName"/>
					<plx:Param dataTypeName="String" dataTypeQualifier="System" name="containerNamespace"/>
					<plx:Param dataTypeName="String" dataTypeQualifier="System" name="containerName"/>
					<plx:Param name="" type="RetVal" dataTypeName="ORMCustomSerializedElementMatch"/>
					<xsl:variable name="forwardToBase">
						<xsl:if test="$ClassOverride">
							<plx:CallInstance name="MapChildElement">
								<plx:CallObject>
									<plx:BaseKeyword/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:Value type="Parameter" data="elementNamespace"/>
								</plx:PassParam>
								<plx:PassParam>
									<plx:Value type="Parameter" data="elementName"/>
								</plx:PassParam>
								<plx:PassParam>
									<plx:Value type="Parameter" data="containerNamespace"/>
								</plx:PassParam>
								<plx:PassParam>
									<plx:Value type="Parameter" data="containerName"/>
								</plx:PassParam>
							</plx:CallInstance>
						</xsl:if>
					</xsl:variable>
					<xsl:choose>
						<xsl:when test="$hasMappedChildElements">
							<plx:Variable name="childElementMappings" dataTypeName="Dictionary">
								<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
								<plx:PassTypeParam dataTypeName="ORMCustomSerializedElementMatch"/>
								<plx:Initialize>
									<plx:CallStatic dataTypeName="{$ClassName}" name="myChildElementMappings" type="Field"/>
								</plx:Initialize>
							</plx:Variable>
							<plx:Condition>
								<plx:Test>
									<plx:Operator type="IdentityEquality">
										<plx:Left>
											<plx:Value type="Local" data="childElementMappings"/>
										</plx:Left>
										<plx:Right>
											<plx:NullObjectKeyword/>
										</plx:Right>
									</plx:Operator>
								</plx:Test>
								<plx:Body>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:Value type="Local" data="childElementMappings"/>
										</plx:Left>
										<plx:Right>
											<plx:CallNew dataTypeName="Dictionary">
												<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
												<plx:PassTypeParam dataTypeName="ORMCustomSerializedElementMatch"/>
											</plx:CallNew>
										</plx:Right>
									</plx:Operator>
									<plx:Variable name="match" dataTypeName="ORMCustomSerializedElementMatch">
										<plx:Initialize>
											<plx:CallNew dataTypeName="ORMCustomSerializedElementMatch"/>
										</plx:Initialize>
									</plx:Variable>
									<xsl:copy-of select="$mapChildElementBody"/>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:CallStatic dataTypeName="{$ClassName}" name="myChildElementMappings" type="Field"/>
										</plx:Left>
										<plx:Right>
											<plx:Value type="Local" data="childElementMappings"/>
										</plx:Right>
									</plx:Operator>
								</plx:Body>
							</plx:Condition>
							<xsl:if test="se:Link | se:Attribute[@WriteStyle='Element']">
							</xsl:if>
							<plx:Variable name="rVal" dataTypeName="ORMCustomSerializedElementMatch"/>
							<xsl:variable name="lookupCall">
								<plx:CallInstance name="TryGetValue">
									<plx:CallObject>
										<plx:Value type="Local" data="childElementMappings"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:CallStatic name="Concat" dataTypeName="String" dataTypeQualifier="System" type="MethodCall">
											<plx:PassParam>
												<plx:Value type="Local" data="containerNamespace"/>
											</plx:PassParam>
											<plx:PassParam>
												<plx:String>|</plx:String>
											</plx:PassParam>
											<plx:PassParam>
												<plx:Value type="Local" data="containerName"/>
											</plx:PassParam>
											<plx:PassParam>
												<plx:String>|</plx:String>
											</plx:PassParam>
											<plx:PassParam>
												<plx:Value type="Local" data="elementNamespace"/>
											</plx:PassParam>
											<plx:PassParam>
												<plx:String>|</plx:String>
											</plx:PassParam>
											<plx:PassParam>
												<plx:Value type="Local" data="elementName"/>
											</plx:PassParam>
										</plx:CallStatic>
									</plx:PassParam>
									<plx:PassParam passStyle="Out">
										<plx:Value type="Local" data="rVal"/>
									</plx:PassParam>
								</plx:CallInstance>
							</xsl:variable>
							<xsl:choose>
								<xsl:when test="$ClassOverride">
									<plx:Condition>
										<plx:Test>
											<plx:Operator type="BooleanNot">
												<xsl:copy-of select="$lookupCall"/>
											</plx:Operator>
										</plx:Test>
										<plx:Body>
											<plx:Operator type="Assign">
												<plx:Left>
													<plx:Value type="Local" data="rVal"/>
												</plx:Left>
												<plx:Right>
													<xsl:copy-of select="$forwardToBase"/>
												</plx:Right>
											</plx:Operator>
										</plx:Body>
									</plx:Condition>
								</xsl:when>
								<xsl:otherwise>
									<xsl:copy-of select="$lookupCall"/>
								</xsl:otherwise>
							</xsl:choose>
							<plx:Return>
								<plx:Value type="Local" data="rVal"/>
							</plx:Return>
						</xsl:when>
						<xsl:otherwise>
							<plx:Return>
								<xsl:choose>
									<xsl:when test="$ClassOverride">
										<xsl:copy-of select="$forwardToBase"/>
									</xsl:when>
									<xsl:otherwise>
										<plx:DefaultValueOf dataTypeName="ORMCustomSerializedElementMatch"/>
									</xsl:otherwise>
								</xsl:choose>
							</plx:Return>
						</xsl:otherwise>
					</xsl:choose>
				</plx:Function>
			</xsl:if>
			<xsl:variable name="attributes" select="se:Attribute[@WriteStyle='Attribute' or not(@WriteStyle)]"/>
			<xsl:if test="$attributes">
				<plx:Field name="myCustomSerializedAttributes" dataTypeName="Dictionary" visibility="Private" static="true">
					<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
					<plx:PassTypeParam dataTypeName="Guid"/>
				</plx:Field>
			</xsl:if>
			<xsl:if test="$attributes or not($ClassOverride)">
				<plx:Function visibility="Protected" name="MapAttribute" shadow="{$ClassOverride}">
					<plx:InterfaceMember dataTypeName="IORMCustomSerializedElement" member="MapAttribute"/>
					<plx:Param dataTypeName="String" dataTypeQualifier="System" name="xmlNamespace"/>
					<plx:Param dataTypeName="String" dataTypeQualifier="System" name="attributeName"/>
					<plx:Param name="" type="RetVal" dataTypeName="Guid"/>
					<xsl:variable name="forwardToBase">
						<xsl:if test="$ClassOverride">
							<plx:CallInstance name="MapAttribute">
								<plx:CallObject>
									<plx:BaseKeyword/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:Value type="Parameter" data="xmlNamespace"/>
								</plx:PassParam>
								<plx:PassParam>
									<plx:Value type="Parameter" data="attributeName"/>
								</plx:PassParam>
							</plx:CallInstance>
						</xsl:if>
					</xsl:variable>
					<xsl:choose>
						<xsl:when test="$attributes">
							<xsl:variable name="namespaces" select="../se:MetaModel/se:Namespaces/se:Namespace"/>
							<plx:Variable name="customSerializedAttributes" dataTypeName="Dictionary">
								<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
								<plx:PassTypeParam dataTypeName="Guid"/>
								<plx:Initialize>
									<plx:CallStatic name="myCustomSerializedAttributes" dataTypeName="{$ClassName}" type="Field"/>
								</plx:Initialize>
							</plx:Variable>
							<plx:Condition>
								<plx:Test>
									<plx:Operator type="IdentityEquality">
										<plx:Left>
											<plx:Value type="Local" data="customSerializedAttributes"/>
										</plx:Left>
										<plx:Right>
											<plx:NullObjectKeyword/>
										</plx:Right>
									</plx:Operator>
								</plx:Test>
								<plx:Body>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:Value type="Local" data="customSerializedAttributes"/>
										</plx:Left>
										<plx:Right>
											<plx:CallNew dataTypeName="Dictionary">
												<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
												<plx:PassTypeParam dataTypeName="Guid"/>
											</plx:CallNew>
										</plx:Right>
									</plx:Operator>
									<xsl:for-each select="$attributes">
										<plx:CallInstance name="Add">
											<plx:CallObject>
												<plx:Value type="Local" data="customSerializedAttributes"/>
											</plx:CallObject>
											<plx:PassParam>
												<plx:String>
													<xsl:if test="string-length(@Prefix)">
														<!-- For attributes, the lack of a prefix means unqualified. Only concatenate if a namespace is explicitly specified -->
														<plx:String>
															<xsl:call-template name="ResolveNamespace">
																<xsl:with-param name="namespaces" select="$namespaces"/>
																<!-- Use default for prefix parameter -->
															</xsl:call-template>
														</plx:String>
														<plx:String>|</plx:String>
													</xsl:if>
													<plx:String>
														<xsl:choose>
															<xsl:when test="string-length(@Name)">
																<xsl:value-of select="@Name"/>
															</xsl:when>
															<xsl:otherwise>
																<xsl:value-of select="@ID"/>
															</xsl:otherwise>
														</xsl:choose>
													</plx:String>
												</plx:String>
											</plx:PassParam>
											<plx:PassParam>
												<plx:CallStatic name="{@ID}MetaAttributeGuid" dataTypeName="{$ClassName}" type="Field"/>
											</plx:PassParam>
										</plx:CallInstance>
									</xsl:for-each>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:CallStatic name="myCustomSerializedAttributes" dataTypeName="{$ClassName}" type="Field"/>
										</plx:Left>
										<plx:Right>
											<plx:Value type="Local" data="customSerializedAttributes"/>
										</plx:Right>
									</plx:Operator>
								</plx:Body>
							</plx:Condition>
							<plx:Variable name="rVal" dataTypeName="Guid"/>
							<plx:Variable name="key" dataTypeName="String" dataTypeQualifier="System">
								<plx:Initialize>
									<plx:Value type="Parameter" data="attributeName"/>
								</plx:Initialize>
							</plx:Variable>
							<plx:Condition>
								<plx:Test>
									<plx:Operator type="BooleanNot">
										<!-- UNDONE: Play games from Plix until CodeDom can do != -->
										<plx:Operator type="Equality">
											<plx:Left>
												<plx:CallInstance name="Length" type="Property">
													<plx:CallObject>
														<plx:Value type="Parameter" data="xmlNamespace"/>
													</plx:CallObject>
												</plx:CallInstance>
											</plx:Left>
											<plx:Right>
												<plx:Value type="I4" data="0"/>
											</plx:Right>
										</plx:Operator>
									</plx:Operator>
								</plx:Test>
								<plx:Body>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:Value type="Local" data="key"/>
										</plx:Left>
										<plx:Right>
											<plx:CallStatic name="Concat" dataTypeName="String" dataTypeQualifier="System">
												<plx:PassParam>
													<plx:Value type="Parameter" data="xmlNamespace"/>
												</plx:PassParam>
												<plx:PassParam>
													<plx:String>|</plx:String>
												</plx:PassParam>
												<plx:PassParam>
													<plx:Value type="Parameter" data="attributeName"/>
												</plx:PassParam>
											</plx:CallStatic>
										</plx:Right>
									</plx:Operator>
								</plx:Body>
							</plx:Condition>
							<xsl:variable name="lookupCall">
								<plx:CallInstance name="TryGetValue">
									<plx:CallObject>
										<plx:Value type="Local" data="customSerializedAttributes"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Local" data="key"/>
									</plx:PassParam>
									<plx:PassParam passStyle="Out">
										<plx:Value type="Local" data="rVal"/>
									</plx:PassParam>
								</plx:CallInstance>
							</xsl:variable>
							<xsl:choose>
								<xsl:when test="$ClassOverride">
									<plx:Condition>
										<plx:Test>
											<plx:Operator type="BooleanNot">
												<xsl:copy-of select="$lookupCall"/>
											</plx:Operator>
										</plx:Test>
										<plx:Body>
											<plx:Operator type="Assign">
												<plx:Left>
													<plx:Value type="Local" data="rVal"/>
												</plx:Left>
												<plx:Right>
													<xsl:copy-of select="$forwardToBase"/>
												</plx:Right>
											</plx:Operator>
										</plx:Body>
									</plx:Condition>
								</xsl:when>
								<xsl:otherwise>
									<xsl:copy-of select="$lookupCall"/>
								</xsl:otherwise>
							</xsl:choose>
							<plx:Return>
								<plx:Value type="Local" data="rVal"/>
							</plx:Return>
						</xsl:when>
						<xsl:otherwise>
							<plx:Return>
								<xsl:choose>
									<xsl:when test="$ClassOverride">
										<xsl:copy-of select="$forwardToBase"/>
									</xsl:when>
									<xsl:otherwise>
										<plx:DefaultValueOf dataTypeName="Guid"/>
									</xsl:otherwise>
								</xsl:choose>
							</plx:Return>
						</xsl:otherwise>
					</xsl:choose>
				</plx:Function>
			</xsl:if>
		</plx:Class>
	</xsl:template>
	<xsl:template name="ResolveNamespace">
		<xsl:param name="namespaces"/>
		<xsl:param name="prefix" select="@Prefix"/>
		<xsl:choose>
			<xsl:when test="string-length($prefix)">
				<xsl:value-of select="$namespaces[@Prefix=$prefix]/@URI"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$namespaces[@DefaultPrefix='true']/@URI"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="se:MetaModel">
		<xsl:variable name="ModelName" select="@Class"/>
		<plx:Class name="{$ModelName}" visibility="Public" partial="true">
			<plx:ImplementsInterface dataTypeName="IORMCustomSerializedMetaModel"/>
			<xsl:for-each select="se:Namespaces">
				<plx:Property visibility="Protected" name="DefaultElementPrefix" static="true">
					<plx:InterfaceMember dataTypeName="IORMCustomSerializedMetaModel" member="DefaultElementPrefix"/>
					<plx:Param name="" type="RetVal" dataTypeName="String" dataTypeQualifier="System"/>
					<plx:Get>
						<plx:Return>
							<xsl:variable name="defaultElement" select="se:Namespace[@DefaultPrefix='true']"/>
							<xsl:choose>
								<xsl:when test="count($defaultElement)">
									<plx:String>
										<xsl:value-of select="$defaultElement/@Prefix"/>
									</plx:String>
								</xsl:when>
								<xsl:otherwise>
									<plx:NullObjectKeyword/>
								</xsl:otherwise>
							</xsl:choose>
						</plx:Return>
					</plx:Get>
				</plx:Property>
				<plx:Function visibility="Protected" name="GetCustomElementNamespaces" static="true">
					<plx:InterfaceMember dataTypeName="IORMCustomSerializedMetaModel" member="GetCustomElementNamespaces"/>
					<plx:Param name="" type="RetVal" dataTypeName="String" dataTypeQualifier="System">
						<plx:ArrayDescriptor rank="2"/>
					</plx:Param>
					<plx:Variable name="ret" dataTypeQualifier="System" dataTypeName="String" const="true">
						<plx:ArrayDescriptor rank="2"/>
						<plx:Initialize>
							<plx:CallNew type="New" dataTypeName="String" dataTypeQualifier="System">
								<plx:ArrayDescriptor rank="2"/>
								<plx:PassParam>
									<plx:Value type="I4" data="{count(se:Namespace)}"/>
								</plx:PassParam>
								<plx:PassParam>
									<plx:Value type="I4" data="3"/>
								</plx:PassParam>
							</plx:CallNew>
						</plx:Initialize>
					</plx:Variable>
					<xsl:for-each select="se:Namespace">
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:CallInstance name="" type="ArrayIndexer">
									<plx:CallObject>
										<plx:Value type="Local" data="ret"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="I4" data="{position()-1}"/>
									</plx:PassParam>
									<plx:PassParam>
										<plx:Value type="I4" data="0"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Left>
							<plx:Right>
								<plx:String>
									<xsl:value-of select="@Prefix"/>
								</plx:String>
							</plx:Right>
						</plx:Operator>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:CallInstance name="" type="ArrayIndexer">
									<plx:CallObject>
										<plx:Value type="Local" data="ret"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="I4" data="{position()-1}"/>
									</plx:PassParam>
									<plx:PassParam>
										<plx:Value type="I4" data="1"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Left>
							<plx:Right>
								<plx:String>
									<xsl:value-of select="@URI"/>
								</plx:String>
							</plx:Right>
						</plx:Operator>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:CallInstance name="" type="ArrayIndexer">
									<plx:CallObject>
										<plx:Value type="Local" data="ret"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="I4" data="{position()-1}"/>
									</plx:PassParam>
									<plx:PassParam>
										<plx:Value type="I4" data="2"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Left>
							<plx:Right>
								<plx:String>
									<xsl:value-of select="@SchemaFile"/>
								</plx:String>
							</plx:Right>
						</plx:Operator>
					</xsl:for-each>
					<plx:Return>
						<plx:Value type="Local" data="ret"/>
					</plx:Return>
				</plx:Function>
			</xsl:for-each>
			<xsl:variable name="hasOmittedElements" select="0!=count(se:OmittedMetaElements/child::se:*)"/>
			<xsl:if test="$hasOmittedElements">
				<plx:Field name="myCustomSerializationOmissions" dataTypeName="Dictionary" visibility="Private">
					<plx:PassTypeParam dataTypeName="MetaClassInfo"/>
					<plx:PassTypeParam dataTypeName="Object" dataTypeQualifier="System"/>
				</plx:Field>
				<plx:Function name="BuildCustomSerializationOmissions" visibility="Private" static="true">
					<plx:Param name="" type="RetVal" dataTypeName="Dictionary">
						<plx:PassTypeParam dataTypeName="MetaClassInfo"/>
						<plx:PassTypeParam dataTypeName="Object" dataTypeQualifier="System"/>
					</plx:Param>
					<plx:Param name="store" dataTypeName="Store"/>
					<plx:Variable name="retVal" dataTypeName="Dictionary">
						<plx:PassTypeParam dataTypeName="MetaClassInfo"/>
						<plx:PassTypeParam dataTypeName="Object" dataTypeQualifier="System"/>
						<plx:Initialize>
							<plx:CallNew type="New" dataTypeName="Dictionary">
								<plx:PassTypeParam dataTypeName="MetaClassInfo"/>
								<plx:PassTypeParam dataTypeName="Object" dataTypeQualifier="System"/>
							</plx:CallNew>
						</plx:Initialize>
					</plx:Variable>
					<plx:Variable name="dataDir" dataTypeName="MetaDataDirectory">
						<plx:Initialize>
							<plx:CallInstance name="MetaDataDirectory" type="Property">
								<plx:CallObject>
									<plx:Value type="Parameter" data="store"/>
								</plx:CallObject>
							</plx:CallInstance>
						</plx:Initialize>
					</plx:Variable>
					<xsl:for-each select="se:OmittedMetaElements/child::se:*">
						<xsl:variable name="classOrRelationship">
							<xsl:choose>
								<xsl:when test="local-name()='OmitClass'">
									<xsl:text>Class</xsl:text>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Relationship</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:variable>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:CallInstance name="" type="Indexer">
									<plx:CallObject>
										<plx:Value type="Local" data="retVal"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:CallInstance name="FindMeta{$classOrRelationship}">
											<plx:CallObject>
												<plx:Value type="Local" data="dataDir"/>
											</plx:CallObject>
											<plx:PassParam>
												<plx:CallStatic name="Meta{$classOrRelationship}Guid" type="Field" dataTypeName="{@Class}" dataTypeQualifier="{@Namespace}"/>
											</plx:PassParam>
										</plx:CallInstance>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Left>
							<plx:Right>
								<plx:NullObjectKeyword/>
							</plx:Right>
						</plx:Operator>
					</xsl:for-each>
					<plx:Return>
						<plx:Value type="Local" data="retVal"/>
					</plx:Return>
				</plx:Function>
			</xsl:if>
			<plx:Field name="myClassNameMap" dataTypeName="Dictionary" visibility="Private" static="true">
				<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
				<plx:PassTypeParam dataTypeName="Guid"/>
			</plx:Field>
			<plx:Field name="myValidNamespaces" dataTypeName="Collection" visibility="Private" static="true">
				<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
			</plx:Field>
			<plx:Function visibility="Protected" name="ShouldSerializeMetaClass">
				<plx:InterfaceMember dataTypeName="IORMCustomSerializedMetaModel" member="ShouldSerializeMetaClass"/>
				<plx:Param name="" type="RetVal" dataTypeName="Boolean" dataTypeQualifier="System"/>
				<plx:Param name="store" dataTypeName="Store"/>
				<plx:Param name="classInfo" dataTypeName="MetaClassInfo"/>
				<xsl:choose>
					<xsl:when test="$hasOmittedElements">
						<plx:Variable name="omissions" dataTypeName="Dictionary">
							<plx:PassTypeParam dataTypeName="MetaClassInfo"/>
							<plx:PassTypeParam dataTypeName="Object" dataTypeQualifier="System"/>
							<plx:Initialize>
								<plx:CallInstance name="myCustomSerializationOmissions" type="Field">
									<plx:CallObject>
										<plx:ThisKeyword/>
									</plx:CallObject>
								</plx:CallInstance>
							</plx:Initialize>
						</plx:Variable>
						<plx:Condition>
							<plx:Test>
								<plx:Operator type="IdentityEquality">
									<plx:Left>
										<plx:Value type="Local" data="omissions"/>
									</plx:Left>
									<plx:Right>
										<plx:NullObjectKeyword/>
									</plx:Right>
								</plx:Operator>
							</plx:Test>
							<plx:Body>
								<plx:Operator type="Assign">
									<plx:Left>
										<plx:Value type="Local" data="omissions"/>
									</plx:Left>
									<plx:Right>
										<plx:CallStatic name="BuildCustomSerializationOmissions" dataTypeName="{$ModelName}">
											<plx:PassParam>
												<plx:Value type="Parameter" data="store"/>
											</plx:PassParam>
										</plx:CallStatic>
									</plx:Right>
								</plx:Operator>
								<plx:Operator type="Assign">
									<plx:Left>
										<plx:CallInstance name="myCustomSerializationOmissions" type="Field">
											<plx:CallObject>
												<plx:ThisKeyword/>
											</plx:CallObject>
										</plx:CallInstance>
									</plx:Left>
									<plx:Right>
										<plx:Value type="Local" data="omissions"/>
									</plx:Right>
								</plx:Operator>
							</plx:Body>
						</plx:Condition>
						<plx:Return>
							<plx:Operator type="BooleanNot">
								<plx:CallInstance name="ContainsKey">
									<plx:CallObject>
										<plx:Value type="Local" data="omissions"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Parameter" data="classInfo"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Operator>
						</plx:Return>
					</xsl:when>
					<xsl:otherwise>
						<plx:Return>
							<plx:TrueKeyword/>
						</plx:Return>
					</xsl:otherwise>
				</xsl:choose>
			</plx:Function>
			<plx:Function visibility="Protected" name="GetRootElementClasses" static="true">
				<plx:InterfaceMember dataTypeName="IORMCustomSerializedMetaModel" member="GetRootElementClasses"/>
				<plx:Param type="RetVal" name="" dataTypeName="Guid" dataTypeIsSimpleArray="true"/>
				<plx:Return>
					<plx:CallNew dataTypeName="Guid" dataTypeIsSimpleArray="true">
						<xsl:variable name="rootElements" select="se:RootElements/se:RootElement"/>
						<xsl:choose>
							<xsl:when test="$rootElements">
								<plx:ArrayInitializer>
									<xsl:for-each select="$rootElements">
										<plx:PassParam>
											<plx:CallStatic dataTypeName="{@Class}" name="MetaClassGuid" type="Field"/>
										</plx:PassParam>
									</xsl:for-each>
								</plx:ArrayInitializer>
							</xsl:when>
							<xsl:otherwise>
								<plx:PassParam>
									<plx:Value type="I4" data="0"/>
								</plx:PassParam>
							</xsl:otherwise>
						</xsl:choose>
					</plx:CallNew>
				</plx:Return>
			</plx:Function>
			<plx:Function visibility="Protected" name="MapRootElement" static="true">
				<plx:InterfaceMember dataTypeName="IORMCustomSerializedMetaModel" member="MapRootElement"/>
				<plx:Param type="RetVal" name="" dataTypeName="Guid"/>
				<plx:Param name="xmlNamespace" dataTypeName="String" dataTypeQualifier="System"/>
				<plx:Param name="elementName" dataTypeName="String" dataTypeQualifier="System"/>
				<xsl:variable name="namespaces" select="se:Namespaces/se:Namespace"/>
				<xsl:for-each select="se:RootElements/se:RootElement">
					<xsl:variable name="className" select="@Class"/>
					<xsl:variable name="tagName">
						<xsl:choose>
							<xsl:when test="string-length(@Name)">
								<xsl:value-of select="@Name"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="$className"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
					<xsl:variable name="namespace">
						<xsl:call-template name="ResolveNamespace">
							<xsl:with-param name="namespaces" select="$namespaces"/>
							<!-- Use default for prefix parameter -->
						</xsl:call-template>
					</xsl:variable>
					<plx:Condition>
						<plx:Test>
							<plx:Operator type="BooleanAnd">
								<plx:Left>
									<plx:Operator type="Equality">
										<plx:Left>
											<plx:Value type="Parameter" data="elementName"/>
										</plx:Left>
										<plx:Right>
											<plx:String>
												<xsl:value-of select="$tagName"/>
											</plx:String>
										</plx:Right>
									</plx:Operator>
								</plx:Left>
								<plx:Right>
									<plx:Operator type="Equality">
										<plx:Left>
											<plx:Value type="Parameter" data="xmlNamespace"/>
										</plx:Left>
										<plx:Right>
											<plx:String>
												<xsl:value-of select="$namespace"/>
											</plx:String>
										</plx:Right>
									</plx:Operator>
								</plx:Right>
							</plx:Operator>
						</plx:Test>
						<plx:Body>
							<plx:Return>
								<plx:CallStatic dataTypeName="{$className}" name="MetaClassGuid" type="Field"/>
							</plx:Return>
						</plx:Body>
					</plx:Condition>
				</xsl:for-each>
				<plx:Return>
					<plx:DefaultValueOf dataTypeName="Guid"/>
				</plx:Return>
			</plx:Function>
			<plx:Function visibility="Protected" name="MapClassName" static="true">
				<plx:InterfaceMember dataTypeName="IORMCustomSerializedMetaModel" member="MapClassName"/>
				<plx:Param type="RetVal" name="" dataTypeName="Guid"/>
				<plx:Param name="xmlNamespace" dataTypeName="String" dataTypeQualifier="System"/>
				<plx:Param name="elementName" dataTypeName="String" dataTypeQualifier="System"/>
				<plx:Variable name="validNamespaces" dataTypeName="Collection">
					<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
					<plx:Initialize>
						<plx:CallStatic dataTypeName="{$ModelName}" name="myValidNamespaces" type="Field"/>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="classNameMap" dataTypeName="Dictionary">
					<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
					<plx:PassTypeParam dataTypeName="Guid"/>
					<plx:Initialize>
						<plx:CallStatic dataTypeName="{$ModelName}" name="myClassNameMap" type="Field"/>
					</plx:Initialize>
				</plx:Variable>
				<plx:Condition>
					<plx:Test>
						<plx:Operator type="IdentityEquality">
							<plx:Left>
								<plx:Value type="Local" data="validNamespaces"/>
							</plx:Left>
							<plx:Right>
								<plx:NullObjectKeyword/>
							</plx:Right>
						</plx:Operator>
					</plx:Test>
					<plx:Body>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:Value type="Local" data="validNamespaces"/>
							</plx:Left>
							<plx:Right>
								<plx:CallNew dataTypeName="Collection">
									<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
								</plx:CallNew>
							</plx:Right>
						</plx:Operator>
						<xsl:for-each select="se:Namespaces/se:Namespace">
							<plx:CallInstance name="Add" type="MethodCall">
								<plx:CallObject>
									<plx:Value type="Local" data="validNamespaces"/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:String>
										<xsl:value-of select="@URI"/>
									</plx:String>
								</plx:PassParam>
							</plx:CallInstance>
						</xsl:for-each>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:CallStatic dataTypeName="{$ModelName}" name="myValidNamespaces" type="Field"/>
							</plx:Left>
							<plx:Right>
								<plx:Value type="Local" data="validNamespaces"/>
							</plx:Right>
						</plx:Operator>
					</plx:Body>
				</plx:Condition>
				<plx:Condition>
					<plx:Test>
						<plx:Operator type="Equality">
							<plx:Left>
								<plx:Value type="Local" data="classNameMap"/>
							</plx:Left>
							<plx:Right>
								<plx:NullObjectKeyword/>
							</plx:Right>
						</plx:Operator>
					</plx:Test>
					<plx:Body>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:Value type="Local" data="classNameMap"/>
							</plx:Left>
							<plx:Right>
								<plx:CallNew dataTypeName="Dictionary">
									<plx:PassTypeParam dataTypeName="String" dataTypeQualifier="System"/>
									<plx:PassTypeParam dataTypeName="Guid"/>
								</plx:CallNew>
							</plx:Right>
						</plx:Operator>
						<xsl:variable name="LocalNamespace" select="$CustomToolNamespace"/>
						<xsl:for-each select="../se:Element">
							<plx:CallInstance name="Add">
								<plx:CallObject>
									<plx:Value type="Local" data="classNameMap"/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:String>
										<xsl:choose>
											<xsl:when test="string-length(@Name) > 0">
												<xsl:value-of select="@Name"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="@Class"/>
											</xsl:otherwise>
										</xsl:choose>
									</plx:String>
								</plx:PassParam>
								<plx:PassParam>
									<plx:CallStatic name="MetaClassGuid" dataTypeName="{@Class}" type="Property"/>
								</plx:PassParam>
							</plx:CallInstance>
							<!-- Handle the less obvious Conditional Names -->
							<xsl:variable name="className" select="@Class"/>
							<xsl:for-each select="se:ConditionalName">
								<plx:CallInstance name="Add">
									<plx:CallObject>
										<plx:Value type="Local" data="classNameMap"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:String>
											<xsl:value-of select="@Name"/>
										</plx:String>
									</plx:PassParam>
									<plx:PassParam>
										<plx:CallStatic name="MetaClassGuid" dataTypeName="{$className}" type="Property"/>
									</plx:PassParam>
								</plx:CallInstance>
							</xsl:for-each>
						</xsl:for-each>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:CallStatic dataTypeName="{$ModelName}" name="myClassNameMap" type="Field"/>
							</plx:Left>
							<plx:Right>
								<plx:Value type="Local" data="classNameMap"/>
							</plx:Right>
						</plx:Operator>
					</plx:Body>
				</plx:Condition>
				<plx:Condition>
					<plx:Test>
						<plx:Operator type="BooleanAnd">
							<plx:Left>
								<plx:CallInstance name="Contains" type="MethodCall">
									<plx:CallObject>
										<plx:Value type="Local" data="validNamespaces"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Local" data="xmlNamespace"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Left>
							<plx:Right>
								<plx:CallInstance name="ContainsKey" type="MethodCall">
									<plx:CallObject>
										<plx:Value type="Local" data="classNameMap"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Local" data="elementName"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Right>
						</plx:Operator>
					</plx:Test>
					<plx:Body>
						<plx:Return>
							<plx:CallInstance name="" type="Indexer">
								<plx:CallObject>
									<plx:Value type="Local" data="classNameMap"/>
								</plx:CallObject>
								<plx:PassParam>
									<plx:Value type="Local" data="elementName"/>
								</plx:PassParam>
							</plx:CallInstance>
						</plx:Return>
					</plx:Body>
				</plx:Condition>
				<plx:Return>
					<plx:DefaultValueOf dataTypeName="Guid"/>
				</plx:Return>
			</plx:Function>
		</plx:Class>
	</xsl:template>
	<xsl:template name="ReturnORMCustomSerializedElementSupportedOperations">
		<xsl:param name="childElements"/>
		<xsl:param name="element"/>
		<xsl:param name="attributes"/>
		<xsl:param name="links"/>
		<xsl:param name="customSort"/>
		<xsl:param name="mixedTypedAttributes"/>
		<xsl:variable name="supportedOperationsFragment">
			<xsl:if test="$childElements">
				<xsl:element name="SupportedOperation">
					<xsl:text>ChildElementInfo</xsl:text>
				</xsl:element>
			</xsl:if>
			<xsl:if test="$element">
				<xsl:element name="SupportedOperation">
					<xsl:text>ElementInfo</xsl:text>
				</xsl:element>
			</xsl:if>
			<xsl:if test="$attributes">
				<xsl:element name="SupportedOperation">
					<xsl:text>AttributeInfo</xsl:text>
				</xsl:element>
			</xsl:if>
			<xsl:if test="$links">
				<xsl:element name="SupportedOperation">
					<xsl:text>LinkInfo</xsl:text>
				</xsl:element>
			</xsl:if>
			<xsl:if test="$customSort">
				<xsl:element name="SupportedOperation">
					<xsl:text>CustomSortChildRoles</xsl:text>
				</xsl:element>
			</xsl:if>
			<xsl:if test="$mixedTypedAttributes">
				<xsl:element name="SupportedOperation">
					<xsl:text>MixedTypedAttributes</xsl:text>
				</xsl:element>
			</xsl:if>
		</xsl:variable>
		<xsl:variable name="supportedOperations" select="msxsl:node-set($supportedOperationsFragment)"/>
		<xsl:variable name="operationCount" select="count($supportedOperations/child::*)"/>
		<xsl:choose>
			<xsl:when test="$operationCount=0">
				<plx:CallStatic dataTypeName="ORMCustomSerializedElementSupportedOperations" name="None" type="Field"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="$supportedOperations/SupportedOperation">
					<xsl:if test="position()=1">
						<xsl:call-template name="OrTogetherEnumElements">
							<xsl:with-param name="EnumType" select="'ORMCustomSerializedElementSupportedOperations'"></xsl:with-param>
						</xsl:call-template>
					</xsl:if>
				</xsl:for-each>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Or together enum values from the given type. The current state on the initial
	     call should be the position()=1 element inside a for-each context where the elements
		 contain the (unqualified) names of the enum values to or together -->
	<xsl:template name="OrTogetherEnumElements">
		<xsl:param name="EnumType"/>
		<xsl:choose>
			<xsl:when test="position()=last()">
				<plx:CallStatic dataTypeName="{$EnumType}" name="{.}" type="Field"/>
			</xsl:when>
			<xsl:otherwise>
				<plx:Operator type="BitwiseOr">
					<plx:Left>
						<plx:CallStatic dataTypeName="{$EnumType}" name="{.}" type="Field"/>
					</plx:Left>
					<plx:Right>
						<xsl:for-each select="following-sibling::*">
							<xsl:if test="position()=1">
								<xsl:call-template name="OrTogetherEnumElements">
									<xsl:with-param name="EnumType" select="$EnumType"/>
								</xsl:call-template>
							</xsl:if>
						</xsl:for-each>
					</plx:Right>
				</plx:Operator>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="CreateORMCustomSerializedElementInfoNameVariable">
		<xsl:param name="modifier"/>
		<xsl:if test="count(se:ConditionalName)">
			<plx:Variable dataTypeName="String" dataTypeQualifier="System" name="name{$modifier}">
				<plx:Initialize>
					<xsl:choose>
						<xsl:when test="string-length(@Name)">
							<plx:String>
								<xsl:value-of select="@Name"/>
							</plx:String>
						</xsl:when>
						<xsl:otherwise>
							<plx:NullObjectKeyword/>
						</xsl:otherwise>
					</xsl:choose>
				</plx:Initialize>
			</plx:Variable>
			<xsl:for-each select="se:ConditionalName">
				<xsl:if test="position()=1">
					<plx:Condition>
						<plx:Test>
							<xsl:copy-of select="child::*"/>
						</plx:Test>
						<plx:Body>
							<plx:Operator type="Assign">
								<plx:Left>
									<plx:Value type="Local" data="name{$modifier}"/>
								</plx:Left>
								<plx:Right>
									<plx:String>
										<xsl:value-of select="@Name"/>
									</plx:String>
								</plx:Right>
							</plx:Operator>
						</plx:Body>
						<xsl:for-each select="following-sibling::se:ConditionalName">
							<plx:FallbackCondition>
								<plx:Test>
									<xsl:copy-of select="child::*"/>
								</plx:Test>
								<plx:Body>
									<plx:Operator type="Assign">
										<plx:Left>
											<plx:Value type="Local" data="name{$modifier}"/>
										</plx:Left>
										<plx:Right>
											<plx:String>
												<xsl:value-of select="@Name"/>
											</plx:String>
										</plx:Right>
									</plx:Operator>
								</plx:Body>
							</plx:FallbackCondition>
						</xsl:for-each>
					</plx:Condition>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	<xsl:template name="PassORMCustomSerializedElementInfoParams">
		<xsl:param name="modifier"/>
		<plx:PassParam>
			<xsl:choose>
				<xsl:when test="string-length(@Prefix)">
					<plx:String>
						<xsl:value-of select="@Prefix"/>
					</plx:String>
				</xsl:when>
				<xsl:otherwise>
					<plx:NullObjectKeyword/>
				</xsl:otherwise>
			</xsl:choose>
		</plx:PassParam>
		<plx:PassParam>
			<xsl:choose>
				<xsl:when test="count(se:ConditionalName)">
					<plx:Value type="Local" data="name{$modifier}"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<xsl:when test="string-length(@Name)">
							<plx:String>
								<xsl:value-of select="@Name"/>
							</plx:String>
						</xsl:when>
						<xsl:otherwise>
							<plx:NullObjectKeyword/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</plx:PassParam>
		<plx:PassParam>
			<xsl:choose>
				<xsl:when test="string-length(@Namespace)">
					<plx:String>
						<xsl:value-of select="@Namespace"/>
					</plx:String>
				</xsl:when>
				<xsl:otherwise>
					<plx:NullObjectKeyword/>
				</xsl:otherwise>
			</xsl:choose>
		</plx:PassParam>
		<plx:PassParam>
			<plx:CallStatic name="pending" type="Field" dataTypeName="ORMCustomSerializedElementWriteStyle">
				<xsl:attribute name="name">
					<xsl:choose>
						<xsl:when test="string-length(@DoubleTagName)">
							<xsl:text>DoubleTaggedElement</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:choose>
								<xsl:when test="string-length(@WriteStyle)">
									<xsl:value-of select="@WriteStyle"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Element</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
			</plx:CallStatic>
		</plx:PassParam>
		<plx:PassParam>
			<xsl:choose>
				<xsl:when test="string-length(@DoubleTagName)">
					<plx:String>
						<xsl:value-of select="@DoubleTagName"/>
					</plx:String>
				</xsl:when>
				<xsl:otherwise>
					<plx:NullObjectKeyword/>
				</xsl:otherwise>
			</xsl:choose>
		</plx:PassParam>
	</xsl:template>
	<xsl:template name="ReturnORMCustomSerializedElementInfo">
		<xsl:call-template name="CreateORMCustomSerializedElementInfoNameVariable"/>
		<plx:Return>
			<plx:CallNew type="New" dataTypeName="ORMCustomSerializedElementInfo">
				<xsl:call-template name="PassORMCustomSerializedElementInfoParams"/>
			</plx:CallNew>
		</plx:Return>
	</xsl:template>
	<xsl:template name="ReturnORMCustomSerializedAttributeInfo">
		<xsl:for-each select="se:Condition">
			<plx:Condition>
				<plx:Test>
					<xsl:copy-of select="child::*"/>
				</plx:Test>
				<plx:Body>
					<xsl:call-template name="ReturnORMCustomSerializedAttributeInfo"/>
				</plx:Body>
			</plx:Condition>
		</xsl:for-each>
		<plx:Return>
			<plx:CallNew type="New" dataTypeName="ORMCustomSerializedAttributeInfo">
				<plx:PassParam>
					<xsl:choose>
						<xsl:when test="string-length(@Prefix)">
							<plx:String>
								<xsl:value-of select="@Prefix"/>
							</plx:String>
						</xsl:when>
						<xsl:otherwise>
							<plx:NullObjectKeyword/>
						</xsl:otherwise>
					</xsl:choose>
				</plx:PassParam>
				<plx:PassParam>
					<xsl:choose>
						<xsl:when test="string-length(@Name)">
							<plx:String>
								<xsl:value-of select="@Name"/>
							</plx:String>
						</xsl:when>
						<xsl:otherwise>
							<plx:NullObjectKeyword/>
						</xsl:otherwise>
					</xsl:choose>
				</plx:PassParam>
				<plx:PassParam>
					<xsl:choose>
						<xsl:when test="string-length(@Namespace)">
							<plx:String>
								<xsl:value-of select="@Namespace"/>
							</plx:String>
						</xsl:when>
						<xsl:otherwise>
							<plx:NullObjectKeyword/>
						</xsl:otherwise>
					</xsl:choose>
				</plx:PassParam>
				<plx:PassParam>
					<xsl:choose>
						<xsl:when test="@WriteCustomStorage='true'">
							<plx:TrueKeyword/>
						</xsl:when>
						<xsl:otherwise>
							<plx:FalseKeyword/>
						</xsl:otherwise>
					</xsl:choose>
				</plx:PassParam>
				<plx:PassParam>
					<plx:CallStatic name="pending" type="Field" dataTypeName="ORMCustomSerializedAttributeWriteStyle">
						<xsl:attribute name="name">
							<xsl:choose>
								<xsl:when test="string-length(@DoubleTagName)">
									<xsl:text>DoubleTaggedElement</xsl:text>
								</xsl:when>
								<xsl:otherwise>
									<xsl:choose>
										<xsl:when test="string-length(@WriteStyle)">
											<xsl:value-of select="@WriteStyle"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:text>Attribute</xsl:text>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
					</plx:CallStatic>
				</plx:PassParam>
				<plx:PassParam>
					<xsl:choose>
						<xsl:when test="string-length(@DoubleTagName)">
							<plx:String>
								<xsl:value-of select="@DoubleTagName"/>
							</plx:String>
						</xsl:when>
						<xsl:otherwise>
							<plx:NullObjectKeyword/>
						</xsl:otherwise>
					</xsl:choose>
				</plx:PassParam>
			</plx:CallNew>
		</plx:Return>
	</xsl:template>
</xsl:stylesheet>
